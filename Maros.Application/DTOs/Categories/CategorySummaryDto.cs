namespace Maros.Application.DTOs.Categories;

public record CategorySummaryDto(
    Guid Id,
    string Name,
    string Slug,
    decimal? DefaultPrice = null,
    string? SurchargeReason = null,
    decimal? Price = null,
    string? ProductSurchargeReason = null
);
