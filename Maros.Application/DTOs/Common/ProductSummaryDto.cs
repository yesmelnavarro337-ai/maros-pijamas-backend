namespace Maros.Application.DTOs.Common;

public record ProductSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    decimal BasePrice,
    string? ThumbnailUrl,
    List<string> Images,
    // Nombre de categoría y estilo fijo: permiten al frontend aplicar la tarifa
    // exacta por estilo/género en las tarjetas sin abrir el detalle.
    string? CategoryName = null,
    string? StyleName = null
);