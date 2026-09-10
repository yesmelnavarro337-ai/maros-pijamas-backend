using Maros.Application.Common;

namespace Maros.Application.DTOs.Blog;

public class BlogQueryParams : PagedQueryParams
{
    public string? Status { get; set; }
    public string? Category { get; set; }
}