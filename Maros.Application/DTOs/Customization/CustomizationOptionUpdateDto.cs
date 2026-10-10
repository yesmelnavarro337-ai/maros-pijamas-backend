using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Customization;

public record CustomizationOptionUpdateDto(
    [Required, MaxLength(100)] string Name,
    [MaxLength(250)] string? Description,
    [MaxLength(100)] string? Category,
    string? ImageUrl,
    string? ColorHex,
    decimal? PriceModifier,
    bool Active,
    List<Guid>? AssignedOptionIds
);
