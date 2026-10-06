namespace Maros.Application.DTOs.Products;

public record ProductVariantResponseDto(
    Guid Id,
    string Size,
    string ColorName,
    string ColorHex,
    string Sku,
    int Stock,
    bool IsAvailable = true,
    decimal? Price = null,
    string? ImageUrl = null,
    string? StyleName = null,
    string? MaterialName = null
);
