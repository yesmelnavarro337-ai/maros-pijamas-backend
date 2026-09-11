namespace Maros.Application.DTOs.Products;

public record ProductPublicDetailDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    decimal BasePrice,
    string CategoryName,
    Guid CategoryId,
    List<string> Images,
    List<string> Sizes,
    List<ProductColorPublicDto> Colors,
    List<ProductPublicVariantDto> Variants,
    bool Available,
    bool AllowCustomization,
    string DeliveryTime,
    string SeoTitle,
    string SeoDescription,
    string? SeoSocialImageUrl,
    string? SeoAltText,
    List<Guid> CollectionIds
);