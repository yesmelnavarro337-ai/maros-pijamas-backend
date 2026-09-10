using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Blog;

public record BlogPostUpdateDto(
    [Required, MaxLength(200)] string Title,
    [Required, MaxLength(60)] string Category,
    string? CoverImageUrl,
    [Required] string Content,
    [Required] string Status,
    [Required] DateTime PublishDate
);