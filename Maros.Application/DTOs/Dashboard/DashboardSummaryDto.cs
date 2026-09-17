namespace Maros.Application.DTOs.Dashboard;

public record KpiMetricDto(
    int Value,
    int PreviousValue,
    double PercentageChange,
    string TrendText
);

public record DashboardKpiSummaryDto(
    KpiMetricDto TotalQuotations,
    KpiMetricDto TotalCustomers,
    KpiMetricDto TotalProducts,
    KpiMetricDto ActiveCollections
);

public record QuotationTrendPointDto(
    string Day,
    string Date,
    int Total
);

public record QuotationStatusBreakdownDto(
    string Status,
    string Label,
    int Count,
    double Percentage,
    string Color
);

public record RecentQuotationDto(
    Guid Id,
    string Code,
    string CustomerName,
    DateTime CreatedAt,
    string FormattedDate,
    decimal TotalAmount,
    string FormattedTotal,
    string Status,
    string ProductSummary
);

public record LowStockProductDto(
    Guid Id,
    string Name,
    int TotalStock,
    string? ThumbnailUrl
);

public record ActiveSeasonSummaryDto(
    string Name,
    string CollectionName,
    Guid CollectionId,
    DateTime StartDate,
    DateTime EndDate
);

public record DashboardSummaryDto(
    DashboardKpiSummaryDto Kpis,
    List<QuotationTrendPointDto> QuotationTrend,
    List<QuotationStatusBreakdownDto> StatusBreakdown,
    List<RecentQuotationDto> RecentQuotations,
    List<LowStockProductDto> LowStockProducts,
    ActiveSeasonSummaryDto? ActiveSeason
);
