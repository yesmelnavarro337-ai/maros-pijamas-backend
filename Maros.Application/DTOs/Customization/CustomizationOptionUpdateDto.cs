using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Customization;

public record CustomizationOptionUpdateDto(
    [Required, MaxLength(100)] string Name,
    string? ImageUrl,
    string? ColorHex,
    decimal? PriceModifier,
    bool Active
);