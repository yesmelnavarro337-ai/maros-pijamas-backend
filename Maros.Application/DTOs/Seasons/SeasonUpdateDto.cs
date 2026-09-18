using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Seasons;

public record SeasonUpdateDto(
    [Required, MaxLength(150)] string Name,
    [Required] Guid CollectionId,
    [Required] DateTime StartDate,
    [Required] DateTime EndDate,
    [Required, MaxLength(200)] string HeroTitle,
    [Required, MaxLength(300)] string HeroSubtitle,
    string? HeroImageUrl,
    string? BannerImageUrl,
    [Required] SeasonColorsDto Colors,
    [MaxLength(60)] string CtaText,
    [MaxLength(200)] string CtaLink,
    List<Guid> FeaturedProductIds,
    string? Status = null
);
