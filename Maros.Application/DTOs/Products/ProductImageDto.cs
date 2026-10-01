namespace Maros.Application.DTOs.Products;

public record ProductImageDto(
    Guid Id,
    string Url,
    int Order,
    string? ColorHex,
    string? ColorName
);
