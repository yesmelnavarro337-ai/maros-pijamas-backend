namespace Maros.Application.DTOs.Products;

public record ProductPublicVariantDto(
    string Size,
    string ColorName,
    string ColorHex,
    bool Available,
    int Stock,
    decimal? Price
);
