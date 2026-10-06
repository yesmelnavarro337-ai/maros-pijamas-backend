using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class ProductVariant : BaseEntity
{
    /// <summary>Stock por defecto cuando la variante se crea sin especificar unidades (generador masivo).</summary>
    public const int DefaultStock = 100;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string Size { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Stock { get; set; } = DefaultStock;
    public bool IsAvailable { get; set; } = true;
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? StyleName { get; set; }
    public string? MaterialName { get; set; }
}
