namespace Maros.Application.DTOs.Categories;

public record CategoryPublicDto(Guid Id, string Name, string Slug, decimal? DefaultPrice = null, string? SurchargeReason = null);