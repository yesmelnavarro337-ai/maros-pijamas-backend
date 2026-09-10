using Maros.Application.DTOs.Common;

namespace Maros.Application.DTOs.Seasons;

public record SeasonPublicResponseDto(
    string Name,
    string HeroTitle,
    string HeroSubtitle,
    string? HeroImageUrl,
    string? BannerImageUrl,
    SeasonColorsDto Colors,
    string CtaText,
    string CtaLink,
    List<ProductSummaryDto> FeaturedProducts
);