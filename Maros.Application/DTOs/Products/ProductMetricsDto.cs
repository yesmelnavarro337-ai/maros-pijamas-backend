namespace Maros.Application.DTOs.Products;

public record ProductMetricsDto(
    int ActiveProductsCount,
    double ActiveProductsVariationPercentage,
    int LowStockCount,
    int OutOfStockCount
);
