using Maros.Domain.Common;

namespace Maros.Domain.Entities;

/// <summary>
/// Contenido editorial administrable de una sección del Home. Una fila por sección
/// (<c>SectionKey</c> único). Los campos son semánticos y genéricos a propósito: cada
/// sección del Home mapea solo los slots que necesita y el resto queda en null.
/// </summary>
public class HomeSectionContent : BaseEntity
{
    /// <summary>
    /// Clave única de la sección: "personalize", "featured-collection" o "brand-promise".
    /// </summary>
    public string SectionKey { get; set; } = string.Empty;

    /// <summary>
    /// Si la sección debe renderizarse en el Home.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Encabezado principal de la sección.
    /// </summary>
    public string? SectionTitle { get; set; }

    /// <summary>
    /// Bajada corta del encabezado.
    /// </summary>
    public string? SectionSubtitle { get; set; }

    /// <summary>
    /// Etiqueta superior en versalitas (p. ej. "NUESTRA PROMESA").
    /// </summary>
    public string? Eyebrow { get; set; }

    /// <summary>
    /// Párrafo descriptivo del cuerpo de la sección.
    /// </summary>
    public string? BodyText { get; set; }

    /// <summary>
    /// Texto del botón de acción principal.
    /// </summary>
    public string? CtaText { get; set; }

    /// <summary>
    /// Destino del botón de acción principal.
    /// </summary>
    public string? CtaLink { get; set; }

    /// <summary>
    /// Imagen principal de la sección (URL absoluta, típicamente Cloudinary).
    /// </summary>
    public string? MainImageUrl { get; set; }

    /// <summary>
    /// Texto alternativo de la imagen principal.
    /// </summary>
    public string? MainImageAlt { get; set; }

    /// <summary>
    /// Imagen de la tarjeta promocional del bloque de pasos (p. ej. "Personaliza tu
    /// pijama en 4 pasos"). Es un slot independiente de <see cref="MainImageUrl"/>,
    /// que en la sección <c>personalize</c> alimenta el banner del carrusel del Home.
    /// </summary>
    public string? CardImageUrl { get; set; }

    /// <summary>
    /// Imágenes secundarias serializadas como JSON (p. ej. las 2 de "Colección Destacada").
    /// </summary>
    public string? SecondaryImagesJson { get; set; }

    /// <summary>
    /// Etiquetas/pills serializadas como JSON (p. ej. las de "Nuestra Promesa").
    /// </summary>
    public string? TagsJson { get; set; }
}