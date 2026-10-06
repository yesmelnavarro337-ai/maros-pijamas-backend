using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Maros.Application.DTOs.Products;

public record ProductUpdateDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public List<Guid> CategoryIds { get; set; } = new();
    public List<CategoryPriceInputDto>? CategoryPrices { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal BasePrice { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    public bool FeaturedHome { get; set; }
    public bool AllowCustomization { get; set; }

    [MaxLength(100)]
    public string DeliveryTime { get; set; } = string.Empty;

    [MaxLength(200)]
    public string SeoTitle { get; set; } = string.Empty;

    [MaxLength(500)]
    public string SeoDescription { get; set; } = string.Empty;

    public string? SeoSocialImageUrl { get; set; }
    public string? SeoAltText { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<ProductImageUpdateDto>? Images { get; set; }
    public List<ProductVariantInputDto> Variants { get; set; } = new();
    public List<Guid> StyleIds { get; set; } = new();
    public List<Guid> CollectionIds { get; set; } = new();
}
