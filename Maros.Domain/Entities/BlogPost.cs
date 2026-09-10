using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class BlogPost : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // ver nota arriba
    public string? CoverImageUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.Borrador;
    public DateTime PublishDate { get; set; }
}