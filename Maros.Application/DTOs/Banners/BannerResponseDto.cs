namespace Maros.Application.DTOs.Banners;

public record BannerResponseDto(
    Guid Id,
    string Title,
    string? ImageUrl,
    string LinkUrl,
    string Position,
    bool Active,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? SeasonId,
    string? SeasonName,
    Guid? CollectionId,
    string? CollectionName
);