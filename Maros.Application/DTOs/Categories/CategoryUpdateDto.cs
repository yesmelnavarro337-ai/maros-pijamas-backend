using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Categories;

/// <summary>
/// Actualización parcial de una categoría. Los campos ausentes o nulos se interpretan
/// como "sin cambio" y preservan el valor almacenado, con dos excepciones deliberadas:
/// <c>Description</c> e <c>ImageUrl</c> se envían siempre (pueden ser nulos para limpiarlos).
/// </summary>
public record CategoryUpdateDto(
    [Required, MaxLength(100)] string Name,
    string? Slug = null,
    string? Description = null,
    string? ImageUrl = null,
    bool? IsActive = null,
    decimal? DefaultPrice = null,
    string? SurchargeReason = null
);