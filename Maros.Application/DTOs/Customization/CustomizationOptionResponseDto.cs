namespace Maros.Application.DTOs.Customization;

public record CustomizationOptionResponseDto(
    Guid Id,
    string CatalogType,
    string Name,
    string? ImageUrl,
    string? ColorHex,
    decimal? PriceModifier,
    bool Active
);