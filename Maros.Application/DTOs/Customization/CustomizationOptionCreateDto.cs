using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Customization;

public record CustomizationOptionCreateDto(
    [Required] string CatalogType,
    [Required, MaxLength(100)] string Name,
    [MaxLength(250)] string? Description,
    [MaxLength(100)] string? Category,
    string? ImageUrl,
    string? ColorHex,
    decimal? PriceModifier,
    List<Guid>? AssignedOptionIds
);
