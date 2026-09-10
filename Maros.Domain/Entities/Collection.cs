using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class Collection : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string AccentHex { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;

    public ICollection<ProductCollection> ProductCollections { get; set; } = new List<ProductCollection>();
    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}