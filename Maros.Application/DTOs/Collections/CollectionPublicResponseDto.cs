using Maros.Application.DTOs.Common;

namespace Maros.Application.DTOs.Collections;

public record CollectionPublicResponseDto(
    string Name,
    string Description,
    string? CoverImageUrl,
    string AccentHex,
    List<ProductSummaryDto> Products
);