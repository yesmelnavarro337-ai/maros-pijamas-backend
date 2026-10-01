namespace Maros.Application.DTOs.Home;

public record HomeSectionContentDto(
    Guid Id,
    string SectionKey,
    bool Enabled,
    string? SectionTitle,
    string? SectionSubtitle,
    string? Eyebrow,
    string? BodyText,
    string? CtaText,
    string? CtaLink,
    string? MainImageUrl,
    string? MainImageAlt,
    List<HomeSectionImageDto> SecondaryImages,
    List<HomeSectionTagDto> Tags
);
