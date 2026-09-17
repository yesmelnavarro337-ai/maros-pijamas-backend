// New DTOs for Dashboard
namespace Maros.Domain.DTOs.Dashboard;

public record DashboardKpiDto(
    int ActiveQuotesCount,
    int LowStockCount,
    int OutOfStockCount,
    decimal TotalRevenueMonth
);

public record DashboardChartDto(
    IList<string> Labels,
    IList<decimal> Datasets
);

public record DashboardRecentQuotationDto(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt
);
