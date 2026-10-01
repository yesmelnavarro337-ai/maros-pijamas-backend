using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Home;

public record HomeSectionImageDto(
    [Required, MaxLength(500)] string Url,
    [MaxLength(180)] string? Alt = null
);
