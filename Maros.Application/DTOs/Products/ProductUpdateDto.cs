using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Products;

public record ProductUpdateDto(
    [Required, MaxLength(200)] string Name,
    Guid? CategoryId,
    [Required] string Description,
    [Range(0.01, double.MaxValue)] decimal BasePrice,
    [Required] string Status,
    bool FeaturedHome,
    bool AllowCustomization,
    [MaxLength(100)] string DeliveryTime,
    [MaxLength(200)] string SeoTitle,
    [MaxLength(300)] string SeoDescription,
    string? SeoSocialImageUrl,
    string? SeoAltText,
    List<string> ImageUrls,
    List<ProductVariantInputDto> Variants,
    List<Guid> CollectionIds
);