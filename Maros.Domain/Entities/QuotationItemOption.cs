namespace Maros.Domain.Entities;

public class QuotationItemOption
{
    public Guid QuotationItemId { get; set; }
    public QuotationItem QuotationItem { get; set; } = null!;

    public Guid CustomizationOptionId { get; set; }
    public CustomizationOption CustomizationOption { get; set; } = null!;
}