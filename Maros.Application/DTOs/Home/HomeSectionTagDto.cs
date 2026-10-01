using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Home;

/// <summary>
/// Etiqueta/pill de una sección del Home. <paramref name="Icon"/> es una clave
/// lógica (p. ej. "tag", "scissors") que el frontend mapea a su propio set de iconos.
/// </summary>
public record HomeSectionTagDto(
    [Required, MaxLength(80)] string Label,
    [MaxLength(40)] string? Icon = null
);
