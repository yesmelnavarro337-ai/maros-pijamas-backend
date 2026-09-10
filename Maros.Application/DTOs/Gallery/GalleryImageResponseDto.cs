namespace Maros.Application.DTOs.Gallery;

public record GalleryImageResponseDto(
    Guid Id,
    string Url,
    string Category,
    string Caption,
    int Order,
    bool Active
);