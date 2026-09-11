namespace Maros.Application.DTOs.Products;

public record ProductPublicListDto(
    Guid Id,
    string Name,
    string Slug,
    string CategoryName,
    decimal BasePrice,
    string? ThumbnailUrl,
    List<string> Images,
    bool Available,
    List<string> Sizes,
    List<ProductPublicColorDto> Colors
);