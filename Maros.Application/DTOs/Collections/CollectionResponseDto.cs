namespace Maros.Application.DTOs.Collections;

public record CollectionResponseDto(
    Guid Id,
    string Name,
    string Description,
    string? CoverImageUrl,
    string AccentHex,
    bool IsDefault,
    List<Guid> ProductIds
);