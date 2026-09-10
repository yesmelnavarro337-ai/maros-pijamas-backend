namespace Maros.Application.DTOs.Blog;

public record BlogPostPublicDto(
    string Title,
    string Slug,
    string Category,
    string? CoverImageUrl,
    string Content,
    DateTime PublishDate
);