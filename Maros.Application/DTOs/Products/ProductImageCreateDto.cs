namespace Maros.Application.DTOs.Products;

public record ProductImageCreateDto
{
    public string Url { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? ColorHex { get; set; }
    public string? ColorName { get; set; }
}
