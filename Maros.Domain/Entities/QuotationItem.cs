using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class QuotationItem : BaseEntity
{
    public Guid QuotationId { get; set; }
    public Quotation Quotation { get; set; } = null!;

    // Nullable a propósito: si el producto se elimina más adelante,
    // la cotización histórica no debe perderse ni romperse.
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }

    public string Size { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string? EmbroideryText { get; set; }

    public ICollection<QuotationItemOption> SelectedOptions { get; set; } = new List<QuotationItemOption>();
}