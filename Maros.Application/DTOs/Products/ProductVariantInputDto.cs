using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Maros.Domain.Entities;

namespace Maros.Application.DTOs.Products;

public record ProductVariantInputDto
{
    public Guid? Id { get; set; }

    [Required, MaxLength(20)]
    public string Size { get; set; } = string.Empty;

    [Required, MaxLength(60)]
    public string ColorName { get; set; } = string.Empty;

    [Required, MaxLength(7)]
    public string ColorHex { get; set; } = string.Empty;

    [Required, MaxLength(60)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>Si no se envía, se aplica <see cref="ProductVariant.DefaultStock"/>.</summary>
    [Range(0, int.MaxValue)]
    public int? Stock { get; set; }

    public bool? IsAvailable { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    [MaxLength(100)]
    public string? StyleName { get; set; }

    [MaxLength(100)]
    public string? MaterialName { get; set; }

    public ProductVariantInputDto() { }

    [JsonConstructor]
    public ProductVariantInputDto(
        Guid? id,
        string size,
        string colorName,
        string colorHex,
        string sku,
        int? stock = null,
        decimal? price = null,
        string? imageUrl = null,
        string? styleName = null,
        string? materialName = null,
        bool? isAvailable = null)
    {
        Id = id;
        Size = size ?? string.Empty;
        ColorName = colorName ?? string.Empty;
        ColorHex = colorHex ?? string.Empty;
        Sku = sku ?? string.Empty;
        Stock = stock;
        Price = price;
        ImageUrl = imageUrl;
        StyleName = styleName;
        MaterialName = materialName;
        IsAvailable = isAvailable;
    }

    public ProductVariantInputDto(
        string size,
        string colorName,
        string colorHex,
        string sku,
        int? stock = null,
        decimal? price = null,
        string? imageUrl = null,
        string? styleName = null,
        string? materialName = null,
        bool? isAvailable = null)
        : this(null, size, colorName, colorHex, sku, stock, price, imageUrl, styleName, materialName, isAvailable) { }

    /// <summary>Unidades efectivas de la variante, aplicando el stock por defecto.</summary>
    public int ResolveStock() => Stock ?? ProductVariant.DefaultStock;

    /// <summary>Disponibilidad efectiva de la variante; por defecto todas quedan disponibles.</summary>
    public bool ResolveIsAvailable() => IsAvailable ?? true;
}