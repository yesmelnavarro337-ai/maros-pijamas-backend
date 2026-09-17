using System.Globalization;
using Maros.Application.DTOs.Dashboard;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using System.Linq;
using Maros.Domain.DTOs.Dashboard;

namespace Maros.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IQuotationRepository _quotationRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ISeasonRepository _seasonRepository;

    private static readonly string[] MonthNames =
    {
        "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"
    };

    public DashboardService(
        IQuotationRepository quotationRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        ICollectionRepository collectionRepository,
        ISeasonRepository seasonRepository)
    {
        _quotationRepository = quotationRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _collectionRepository = collectionRepository;
        _seasonRepository = seasonRepository;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var periodStart = now.AddDays(-30);
        var prevPeriodStart = now.AddDays(-60);

        // 1. KPIs
        var allQuotations = _quotationRepository.QueryAll().ToList();
        var currentQuotations = allQuotations.Count(q => q.CreatedAt >= periodStart);
        var prevQuotations = allQuotations.Count(q => q.CreatedAt >= prevPeriodStart && q.CreatedAt < periodStart);
        var totalQuotationsCount = allQuotations.Count;

        var allCustomers = _customerRepository.QueryAll().ToList();
        var currentCustomers = allCustomers.Count(c => c.CreatedAt >= periodStart);
        var prevCustomers = allCustomers.Count(c => c.CreatedAt >= prevPeriodStart && c.CreatedAt < periodStart);
        var totalCustomersCount = allCustomers.Count;

        var allProducts = _productRepository.QueryAll().ToList();

        var activeProducts = allProducts.Where(p => p.Status == ProductStatus.Activo && !p.IsDeleted).ToList();
        var currentProducts = activeProducts.Count(p => p.CreatedAt >= periodStart);
        var prevProducts = activeProducts.Count(p => p.CreatedAt >= prevPeriodStart && p.CreatedAt < periodStart);
        var totalProductsCount = activeProducts.Count;

        var collections = await _collectionRepository.GetAllAsync();
        var currentCollections = collections.Count(c => c.CreatedAt >= periodStart);
        var prevCollections = collections.Count(c => c.CreatedAt >= prevPeriodStart && c.CreatedAt < periodStart);
        var totalCollectionsCount = collections.Count;

        var kpis = new DashboardKpiSummaryDto(
            TotalQuotations: BuildKpiMetric(totalQuotationsCount, currentQuotations, prevQuotations),
            TotalCustomers: BuildKpiMetric(totalCustomersCount, currentCustomers, prevCustomers),
            TotalProducts: BuildKpiMetric(totalProductsCount, currentProducts, prevProducts),
            ActiveCollections: BuildKpiMetric(totalCollectionsCount, currentCollections, prevCollections)
        );

        // 2. Trend (7d default)
        var quotationTrend = GetQuotationTrendInternal(allQuotations, "7d");

        // 3. Status Breakdown
        var statusBreakdown = BuildStatusBreakdown(allQuotations);

        // 4. Recent Quotations
        var recentQuotationsRaw = _quotationRepository.QueryAll()
            .OrderByDescending(q => q.CreatedAt)
            .Take(5)
            .ToList();

        var recentQuotations = recentQuotationsRaw.Select(q =>
        {
            var totalAmount = q.Items.Sum(i =>
            {
                var basePrice = i.Product?.BasePrice ?? 0m;
                var optionsPrice = i.SelectedOptions.Sum(o => o.CustomizationOption?.PriceModifier ?? 0m);
                return (basePrice + optionsPrice) * i.Quantity;
            });

            var itemSummary = q.Items.Count switch
            {
                0 => "Sin productos",
                1 => q.Items.First().Product?.Name ?? "Producto",
                _ => $"{q.Items.First().Product?.Name ?? "Producto"} +{q.Items.Count - 1} más"
            };

            var uiStatus = MapToUiStatus(q.Status);

            return new RecentQuotationDto(
                Id: q.Id,
                Code: $"CZ-{q.Id.ToString()[..4].ToUpper()}",
                CustomerName: q.Customer?.Name ?? "Cliente N/A",
                CreatedAt: q.CreatedAt,
                FormattedDate: q.CreatedAt.ToString("d MMM yyyy", CultureInfo.GetCultureInfo("es-ES")),
                TotalAmount: totalAmount,
                FormattedTotal: $"$ {totalAmount:N0}".Replace(",", "."),
                Status: uiStatus,
                ProductSummary: itemSummary
            );
        }).ToList();

        // 5. Low Stock Products
        var lowStockProducts = activeProducts
            .Select(p => new LowStockProductDto(
                Id: p.Id,
                Name: p.Name,
                TotalStock: p.Variants.Sum(v => v.Stock),
                ThumbnailUrl: p.Images.FirstOrDefault()?.Url
            ))
            .OrderBy(p => p.TotalStock)
            .Take(5)
            .ToList();

        // 6. Active Season
        var activeSeasonEntity = await _seasonRepository.GetActiveAsync();
        ActiveSeasonSummaryDto? activeSeason = null;
        if (activeSeasonEntity != null)
        {
            activeSeason = new ActiveSeasonSummaryDto(
                Name: activeSeasonEntity.Name,
                CollectionName: activeSeasonEntity.Collection?.Name ?? string.Empty,
                CollectionId: activeSeasonEntity.CollectionId,
                StartDate: activeSeasonEntity.StartDate,
                EndDate: activeSeasonEntity.EndDate
            );
        }

        return new DashboardSummaryDto(
            Kpis: kpis,
            QuotationTrend: quotationTrend,
            StatusBreakdown: statusBreakdown,
            RecentQuotations: recentQuotations,
            LowStockProducts: lowStockProducts,
            ActiveSeason: activeSeason
        );
    }

    public Task<List<QuotationTrendPointDto>> GetQuotationTrendAsync(string period)
    {
        var allQuotations = _quotationRepository.QueryAll().ToList();
        var trend = GetQuotationTrendInternal(allQuotations, period);
        return Task.FromResult(trend);
    }

        // New Dashboard API methods
        public async Task<DashboardKpiDto> GetKpisAsync(string? period = null)
        {
            var summary = await GetSummaryAsync();
            var k = summary.Kpis;
            return new DashboardKpiDto(
                ActiveQuotesCount: k.TotalQuotations.Value,
                LowStockCount: 0,
                OutOfStockCount: 0,
                TotalRevenueMonth: 0m);
        }

        public async Task<DashboardChartDto> GetSalesChartAsync(string? period = null)
        {
            // Placeholder implementation – replace with real chart data aggregation
            return new DashboardChartDto(Array.Empty<string>(), Array.Empty<decimal>());
        }

        public async Task<DashboardChartDto> GetCategoryChartAsync()
        {
            // Placeholder implementation – replace with real category aggregation
            return new DashboardChartDto(Array.Empty<string>(), Array.Empty<decimal>());
        }

        public async Task<IReadOnlyList<DashboardRecentQuotationDto>> GetRecentQuotationsAsync(int count = 5)
        {
            var summary = await GetSummaryAsync();
            var recent = summary.RecentQuotations.Take(count).Select(q => new DashboardRecentQuotationDto(
                Id: q.Id,
                CustomerName: q.CustomerName,
                TotalAmount: q.TotalAmount,
                Status: q.Status,
                CreatedAt: q.CreatedAt)).ToList();
            return recent;
        }

    private static List<QuotationTrendPointDto> GetQuotationTrendInternal(List<Quotation> quotations, string period)
    {
        var result = new List<QuotationTrendPointDto>();
        var today = DateTime.UtcNow.Date;

        if (period.Equals("1y", StringComparison.OrdinalIgnoreCase) || period.Equals("thisyear", StringComparison.OrdinalIgnoreCase))
        {
            var currentYear = today.Year;
            for (var month = 1; month <= 12; month++)
            {
                var count = quotations.Count(q => q.CreatedAt.Year == currentYear && q.CreatedAt.Month == month);
                var monthLabel = MonthNames[month - 1];
                result.Add(new QuotationTrendPointDto(
                    Day: monthLabel,
                    Date: new DateTime(currentYear, month, 1).ToString("yyyy-MM-dd"),
                    Total: count
                ));
            }
        }
        else if (period.Equals("30d", StringComparison.OrdinalIgnoreCase))
        {
            for (var i = 29; i >= 0; i--)
            {
                var targetDate = today.AddDays(-i);
                var count = quotations.Count(q => q.CreatedAt.Date == targetDate);
                var dayLabel = targetDate.ToString("d MMM", CultureInfo.GetCultureInfo("es-ES"));
                result.Add(new QuotationTrendPointDto(
                    Day: dayLabel,
                    Date: targetDate.ToString("yyyy-MM-dd"),
                    Total: count
                ));
            }
        }
        else // default "7d"
        {
            for (var i = 6; i >= 0; i--)
            {
                var targetDate = today.AddDays(-i);
                var count = quotations.Count(q => q.CreatedAt.Date == targetDate);
                var dayLabel = targetDate.ToString("d MMM", CultureInfo.GetCultureInfo("es-ES"));
                result.Add(new QuotationTrendPointDto(
                    Day: dayLabel,
                    Date: targetDate.ToString("yyyy-MM-dd"),
                    Total: count
                ));
            }
        }

        return result;
    }

    private static KpiMetricDto BuildKpiMetric(int totalValue, int currentPeriodCount, int prevPeriodCount)
    {
        double percentageChange;
        if (prevPeriodCount == 0)
        {
            percentageChange = currentPeriodCount > 0 ? 100.0 : 0.0;
        }
        else
        {
            percentageChange = Math.Round(((double)(currentPeriodCount - prevPeriodCount) / prevPeriodCount) * 100.0, 1);
        }

        var trendText = percentageChange >= 0
            ? $"{percentageChange:0.#}% este mes"
            : $"{Math.Abs(percentageChange):0.#}% este mes";

        return new KpiMetricDto(
            Value: totalValue,
            PreviousValue: prevPeriodCount,
            PercentageChange: percentageChange,
            TrendText: trendText
        );
    }

    private static List<QuotationStatusBreakdownDto> BuildStatusBreakdown(List<Quotation> quotations)
    {
        var total = quotations.Count;

        var pendienteCount = quotations.Count(q => q.Status == QuotationStatus.Nueva);
        var enProcesoCount = quotations.Count(q => q.Status == QuotationStatus.EnRevision || q.Status == QuotationStatus.Contactada);
        var respondidaCount = quotations.Count(q => q.Status == QuotationStatus.Cotizada || q.Status == QuotationStatus.Aceptada);
        var canceladaCount = quotations.Count(q => q.Status == QuotationStatus.Rechazada || q.Status == QuotationStatus.Archivada);

        double CalcPct(int count) => total > 0 ? Math.Round(((double)count / total) * 100.0, 1) : 0.0;

        return new List<QuotationStatusBreakdownDto>
        {
            new("Pendiente", "Pendientes", pendienteCount, CalcPct(pendienteCount), "#6A5E39"),
            new("En proceso", "En proceso", enProcesoCount, CalcPct(enProcesoCount), "#748CAB"),
            new("Respondida", "Respondidas", respondidaCount, CalcPct(respondidaCount), "#D4B982"),
            new("Cancelada", "Canceladas", canceladaCount, CalcPct(canceladaCount), "#F0A6A6")
        };
    }

    private static string MapToUiStatus(QuotationStatus status) => status switch
    {
        QuotationStatus.Nueva => "Pendiente",
        QuotationStatus.EnRevision or QuotationStatus.Contactada => "En proceso",
        QuotationStatus.Cotizada or QuotationStatus.Aceptada => "Respondida",
        QuotationStatus.Rechazada or QuotationStatus.Archivada => "Cancelada",
        _ => "Pendiente"
    };
}
