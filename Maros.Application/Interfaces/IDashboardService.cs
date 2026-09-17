using Maros.Application.DTOs.Dashboard;
using Maros.Domain.DTOs.Dashboard;

namespace Maros.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
    Task<List<QuotationTrendPointDto>> GetQuotationTrendAsync(string period);
    Task<DashboardKpiDto> GetKpisAsync(string? period = null);
    Task<DashboardChartDto> GetSalesChartAsync(string? period = null);
    Task<DashboardChartDto> GetCategoryChartAsync();
    Task<IReadOnlyList<DashboardRecentQuotationDto>> GetRecentQuotationsAsync(int count = 5);
}
