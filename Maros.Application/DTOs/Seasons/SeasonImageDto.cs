using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Seasons;

public record SeasonImageDto(
    Guid Id,
    string ImageUrl,
    int Order,
    bool IsPrimary
);

/// <summary>
/// Imagen recibida desde el admin. <see cref="Id"/> es null cuando es nueva; si viene
/// informado se reutiliza la fila existente en lugar de borrarla y recrearla.
/// </summary>
public record SeasonImageInputDto(
    Guid? Id,
    [Required] string ImageUrl,
    int? Order = null,
    bool IsPrimary = false
);