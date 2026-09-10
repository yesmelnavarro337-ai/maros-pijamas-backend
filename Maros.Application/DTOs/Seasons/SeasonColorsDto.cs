using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Seasons;

public record SeasonColorsDto(
    [Required, MaxLength(7)] string Primary,
    [Required, MaxLength(7)] string Accent,
    [Required, MaxLength(7)] string Background
);