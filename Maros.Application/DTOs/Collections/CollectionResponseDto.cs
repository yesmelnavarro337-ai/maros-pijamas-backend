namespace Maros.Application.DTOs.Collections;

public record CollectionResponseDto(
    Guid Id,
    string Name,
    string Description,
    string? CoverImageUrl,
    string AccentHex,
    bool IsDefault,
    List<Guid> ProductIds,
    string? SeasonName = null,
    bool IsActive = true,
    int ProductsCount = 0,
    DateTime? UpdatedAt = null
);