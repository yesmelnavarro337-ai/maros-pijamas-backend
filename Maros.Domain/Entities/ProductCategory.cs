namespace Maros.Domain.Entities;

public class ProductCategory
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    /// <summary>Precio específico del producto en esta categoría. Si es null, se usa BasePrice.</summary>
    public decimal? Price { get; set; }

    /// <summary>Motivo del recargo respecto al precio base (ej: "Incluye bordado artesanal").</summary>
    public string? SurchargeReason { get; set; }
}
