namespace Maros.Application.DTOs.Customization;

public record CustomizationOptionPublicDto(
    Guid Id,
    string Name,
    string? ImageUrl,
    string? ColorHex,
    decimal? PriceModifier
);

public record CustomizationCatalogPublicDto(
    Dictionary<string, List<CustomizationOptionPublicDto>> Catalogs
);