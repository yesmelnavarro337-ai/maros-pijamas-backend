using Maros.Application.DTOs.Categories;

namespace Maros.Application.DTOs.Products;

public record ProductPublicListDto(
    Guid Id,
    string Name,
    string Slug,
    string CategoryName,
    List<Guid> CategoryIds,
    List<CategorySummaryDto> Categories,
    decimal BasePrice,
    string? ThumbnailUrl,
    List<string> Images,
    bool Available,
    List<string> Sizes,
    List<ProductPublicColorDto> Colors,
    // Estilo fijo predominante: las tarjetas del catálogo necesitan la tarifa
    // exacta por estilo (Hombre/Mujer) en lugar de la de la categoría.
    string? StyleName = null
);
