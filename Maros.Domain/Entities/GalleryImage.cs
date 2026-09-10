using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class GalleryImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public GalleryCategory Category { get; set; }
    public string Caption { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool Active { get; set; } = true;
}