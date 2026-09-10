using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Customization;

public record CustomizationOptionCreateDto(
    [Required] string CatalogType,
    [Required, MaxLength(100)] string Name,
    string? ImageUrl,
    string? ColorHex,
    decimal? PriceModifier
);