namespace Maros.Application.DTOs.PageHeaders;

public record PageHeaderDto(
    string PageKey,
    string Title,
    string Subtitle,
    string? BackgroundImageUrl,
    string? PrimaryButtonText,
    string? PrimaryButtonLink,
    string? SecondaryButtonText,
    string? SecondaryButtonLink,
    string TextColor,
    int OverlayOpacity
);