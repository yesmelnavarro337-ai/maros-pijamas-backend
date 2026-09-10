namespace Maros.Application.DTOs.Blog;

public record BlogPostResponseDto(
    Guid Id,
    string Title,
    string Slug,
    string Category,
    string? CoverImageUrl,
    string Content,
    string Status,
    DateTime PublishDate
);