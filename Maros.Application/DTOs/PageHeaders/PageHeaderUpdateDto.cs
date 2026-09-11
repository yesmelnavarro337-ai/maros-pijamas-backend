using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.PageHeaders;

public record PageHeaderUpdateDto(
    [Required, MaxLength(200)] string Title,
    [MaxLength(500)] string Subtitle,
    [MaxLength(500)] string? BackgroundImageUrl,
    [MaxLength(80)] string? PrimaryButtonText,
    [MaxLength(300)] string? PrimaryButtonLink,
    [MaxLength(80)] string? SecondaryButtonText,
    [MaxLength(300)] string? SecondaryButtonLink,
    string TextColor,
    [Range(0, 90)] int OverlayOpacity
);