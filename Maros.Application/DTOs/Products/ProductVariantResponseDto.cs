namespace Maros.Application.DTOs.Products;

public record ProductVariantResponseDto(
    Guid Id,
    string Size,
    string ColorName,
    string ColorHex,
    string Sku,
    int Stock,
    string? ImageUrl
);