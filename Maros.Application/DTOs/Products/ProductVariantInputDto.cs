using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Products;

public record ProductVariantInputDto(
    [Required, MaxLength(20)] string Size,
    [Required, MaxLength(60)] string ColorName,
    [Required, MaxLength(7)] string ColorHex,
    [Required, MaxLength(60)] string Sku,
    [Range(0, int.MaxValue)] int Stock,
    string? ImageUrl
);