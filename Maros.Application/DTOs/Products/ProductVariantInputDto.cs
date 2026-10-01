using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    public ProductVariantInputDto() { }

    [JsonConstructor]
    public ProductVariantInputDto(
        Guid? id,
        string size,
        string colorName,
        string colorHex,
        string sku,
        int stock,
        decimal? price,
        string? imageUrl)
    {
        Id = id;
        Size = size ?? string.Empty;
        ColorName = colorName ?? string.Empty;
        ColorHex = colorHex ?? string.Empty;
        Sku = sku ?? string.Empty;
        Stock = stock;
        Price = price;
        ImageUrl = imageUrl;
    }

    public ProductVariantInputDto(
        string size,
        string colorName,
        string colorHex,
        string sku,
        int stock,
        decimal? price,
        string? imageUrl)
        : this(null, size, colorName, colorHex, sku, stock, price, imageUrl) { }
}
