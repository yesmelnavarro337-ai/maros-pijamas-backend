using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Products;

public record ProductCreateDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public List<Guid> CategoryIds { get; set; } = new();

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

    [MaxLength(300)]
    public string SeoDescription { get; set; } = string.Empty;

    public string? SeoSocialImageUrl { get; set; }
    public string? SeoAltText { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<ProductVariantInputDto> Variants { get; set; } = new();
    public List<Guid> CollectionIds { get; set; } = new();
}
