using Maros.Application.Common;

namespace Maros.Application.DTOs.Gallery;

public class GalleryQueryParams : PagedQueryParams
{
    public string? Category { get; set; }
}