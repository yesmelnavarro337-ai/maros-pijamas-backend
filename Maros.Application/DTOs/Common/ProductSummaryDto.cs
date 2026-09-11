namespace Maros.Application.DTOs.Common;

public record ProductSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    decimal BasePrice,
    string? ThumbnailUrl,
    List<string> Images
);