namespace Maros.Application.DTOs.Products;

public record ProductResponseDto(
    Guid Id,
    string Name,
    string Slug,
    Guid? CategoryId,
    string CategoryName,
    string Description,
    decimal BasePrice,
    string Status,
    bool FeaturedHome,
    bool AllowCustomization,
    string DeliveryTime,
    string SeoTitle,
    string SeoDescription,
    string SeoSlug,
    string? SeoSocialImageUrl,
    string? SeoAltText,
    List<string> Images,
    List<ProductVariantResponseDto> Variants,
    List<Guid> CollectionIds,
    DateTime CreatedAt,
    string Sku = "",
    int TotalStock = 0,
    string SeasonName = "Sin temporada",
    string? ImageUrl = null
);