namespace Maros.Application.DTOs.Products;

public record CategoryPriceInputDto
{
    public Guid CategoryId { get; set; }
    public decimal? Price { get; set; }
    public string? SurchargeReason { get; set; }
}
