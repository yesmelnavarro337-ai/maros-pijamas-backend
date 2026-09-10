using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class Season : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SeasonStatus Status { get; set; } = SeasonStatus.Borrador;

    public Guid CollectionId { get; set; }
    public Collection Collection { get; set; } = null!;

    public string HeroTitle { get; set; } = string.Empty;
    public string HeroSubtitle { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }
    public string? BannerImageUrl { get; set; }

    public string ColorPrimary { get; set; } = string.Empty;
    public string ColorAccent { get; set; } = string.Empty;
    public string ColorBackground { get; set; } = string.Empty;

    public string CtaText { get; set; } = string.Empty;
    public string CtaLink { get; set; } = string.Empty;

    public ICollection<SeasonFeaturedProduct> FeaturedProducts { get; set; } = new List<SeasonFeaturedProduct>();
}