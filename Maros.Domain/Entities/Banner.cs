using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class Banner : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string LinkUrl { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public bool Active { get; set; } = true;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Guid? SeasonId { get; set; }
    public Season? Season { get; set; }

    public Guid? CollectionId { get; set; }
    public Collection? Collection { get; set; }
}