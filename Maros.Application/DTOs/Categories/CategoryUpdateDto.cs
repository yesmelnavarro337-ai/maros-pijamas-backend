using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Categories;

public record CategoryUpdateDto(
    [Required, MaxLength(100)] string Name,
    string? Slug = null,
    string? Description = null,
    string? ImageUrl = null,
    bool IsActive = true
);