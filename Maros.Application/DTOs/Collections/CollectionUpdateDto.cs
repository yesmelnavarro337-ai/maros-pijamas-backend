using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Collections;

public record CollectionUpdateDto(
    [Required, MaxLength(100)] string Name,
    [Required] string Description,
    string? CoverImageUrl,
    [Required, MaxLength(7)] string AccentHex,
    List<Guid> ProductIds
);