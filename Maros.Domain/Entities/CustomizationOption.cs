using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class CustomizationOption : BaseEntity
{
    public CustomizationCatalogType CatalogType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? ColorHex { get; set; }
    public decimal? PriceModifier { get; set; }
    public bool Active { get; set; } = true;
}