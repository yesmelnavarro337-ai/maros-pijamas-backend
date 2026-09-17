namespace Maros.Application.DTOs.Seasons;

public record SeasonResponseDto(
    Guid Id,
    string Name,
    string Slug,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    Guid CollectionId,
    string CollectionName,
    string HeroTitle,
    string HeroSubtitle,
    string? HeroImageUrl,
    string? BannerImageUrl,
    SeasonColorsDto Colors,
    string CtaText,
    string CtaLink,
    List<Guid> FeaturedProductIds,
    bool IsActive = false,
    string? CoverImageUrl = null,
    int ProductsCount = 0
);