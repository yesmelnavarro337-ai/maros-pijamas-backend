namespace Maros.Application.DTOs.Products;

public record ProductFeaturedSelectionDto(
    Guid Id,
    string Name,
    string Slug,
    decimal BasePrice,
    string? ImageUrl,
    int? CatalogOrder
);