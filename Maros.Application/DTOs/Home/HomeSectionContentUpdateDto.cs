using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Home;

/// <summary>
/// Reemplazo completo del contenido de una sección del Home (semántica PUT).
/// El formulario del admin siempre envía todos los campos, así que un valor nulo
/// significa "vaciar este campo".
/// </summary>
public record HomeSectionContentUpdateDto(
    bool? Enabled = null,
    [MaxLength(200)] string? SectionTitle = null,
    [MaxLength(500)] string? SectionSubtitle = null,
    [MaxLength(80)] string? Eyebrow = null,
    [MaxLength(600)] string? BodyText = null,
    [MaxLength(80)] string? CtaText = null,
    [MaxLength(300)] string? CtaLink = null,
    [MaxLength(500)] string? MainImageUrl = null,
    [MaxLength(180)] string? MainImageAlt = null,
    [MaxLength(500)] string? CardImageUrl = null,
    List<HomeSectionImageDto>? SecondaryImages = null,
    List<HomeSectionTagDto>? Tags = null
);
