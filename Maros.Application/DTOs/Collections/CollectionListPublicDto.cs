namespace Maros.Application.DTOs.Collections;

public record CollectionListPublicDto(
    Guid Id,
    string Name,
    string Description,
    string? CoverImageUrl,
    string AccentHex,
    int ProductCount
);