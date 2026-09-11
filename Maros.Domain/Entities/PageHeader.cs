using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class PageHeader : BaseEntity
{
    public string PageKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string? BackgroundImageUrl { get; set; }
    public string? PrimaryButtonText { get; set; }
    public string? PrimaryButtonLink { get; set; }
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonLink { get; set; }
    public string TextColor { get; set; } = "#F9F6F0";
    public int OverlayOpacity { get; set; } = 40;
}