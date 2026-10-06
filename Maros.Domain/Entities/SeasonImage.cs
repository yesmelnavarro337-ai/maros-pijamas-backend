using Maros.Domain.Common;

namespace Maros.Domain.Entities;

/// <summary>
/// Imagen relacionable de una temporada. La portada de la temporada se marca con
/// <see cref="IsPrimary"/> y solo puede existir una por temporada: la base de datos
/// lo garantiza con un índice único filtrado y SeasonService lo normaliza antes de
/// guardar. El campo <see cref="Order"/> define la posición en el carrusel del Home.
/// </summary>
public class SeasonImage : BaseEntity
{
    public Guid SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool IsPrimary { get; set; }
}