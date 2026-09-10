namespace Maros.Domain.Entities;

public class SeasonFeaturedProduct
{
    public Guid SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}