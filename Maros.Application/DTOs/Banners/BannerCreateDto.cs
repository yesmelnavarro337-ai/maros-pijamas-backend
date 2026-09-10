using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Banners;

public record BannerCreateDto(
    [Required, MaxLength(200)] string Title,
    string? ImageUrl,
    [MaxLength(300)] string LinkUrl,
    [Required, MaxLength(60)] string Position,
    bool Active,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? SeasonId,
    Guid? CollectionId
);