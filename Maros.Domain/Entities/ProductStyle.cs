namespace Maros.Domain.Entities;

public class ProductStyle
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid StyleId { get; set; }
    public Style Style { get; set; } = null!;
}