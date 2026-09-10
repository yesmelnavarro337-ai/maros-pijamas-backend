using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Borrador;
    public bool FeaturedHome { get; set; }
    public bool AllowCustomization { get; set; } = true;
    public string DeliveryTime { get; set; } = string.Empty;

    // SEO
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string SeoSlug { get; set; } = string.Empty;
    public string? SeoSocialImageUrl { get; set; }
    public string? SeoAltText { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductCollection> ProductCollections { get; set; } = new List<ProductCollection>();
}