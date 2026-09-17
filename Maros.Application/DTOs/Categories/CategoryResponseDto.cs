namespace Maros.Application.DTOs.Categories;

public record CategoryResponseDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    bool IsActive,
    int ProductsCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
