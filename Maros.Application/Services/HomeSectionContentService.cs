using System.Text.Json;
using Maros.Application.Common;
using Maros.Application.DTOs.Home;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class HomeSectionContentService : IHomeSectionContentService
{
    /// <summary>
    /// Claves de sección soportadas. Se validan en la escritura para que un typo
    /// desde el admin no cree una fila muerta que el Home nunca va a leer.
    /// </summary>
    private static readonly HashSet<string> KnownSectionKeys =
    [
        "personalize",
        "featured-collection",
        "brand-promise",
    ];

    private const int MaxSecondaryImages = 6;
    private const int MaxTags = 12;

    private readonly IHomeSectionContentRepository _repository;

    public HomeSectionContentService(IHomeSectionContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HomeSectionContentDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<List<HomeSectionContentDto>> GetPublicAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Where(s => s.Enabled).Select(ToDto).ToList();
    }

    public async Task<HomeSectionContentDto> UpdateAsync(string sectionKey, HomeSectionContentUpdateDto request)
    {
        var key = (sectionKey ?? string.Empty).Trim().ToLowerInvariant();

        if (!KnownSectionKeys.Contains(key))
            throw new AppException($"Sección '{sectionKey}' no reconocida.", 400);

        ValidateSecondaryImages(request.SecondaryImages);
        ValidateTags(request.Tags);

        var content = await _repository.GetBySectionKeyAsync(key);
        if (content is null)
        {
            // Upsert: si el admin edita antes de que corra el seed, la fila se crea aquí
            // en vez de devolver 404 y obligar a una migración manual.
            content = new HomeSectionContent { SectionKey = key };
            await _repository.AddAsync(content);
        }

        content.Enabled = request.Enabled ?? true;
        content.SectionTitle = request.SectionTitle;
        content.SectionSubtitle = request.SectionSubtitle;
        content.Eyebrow = request.Eyebrow;
        content.BodyText = request.BodyText;
        content.CtaText = request.CtaText;
        content.CtaLink = request.CtaLink;
        content.MainImageUrl = request.MainImageUrl;
        content.MainImageAlt = request.MainImageAlt;
        content.CardImageUrl = request.CardImageUrl;
        content.SecondaryImagesJson = Serialize(request.SecondaryImages);
        content.TagsJson = Serialize(request.Tags);
        content.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return ToDto(content);
    }

    private static void ValidateSecondaryImages(List<HomeSectionImageDto>? images)
    {
        if (images is null) return;

        if (images.Count > MaxSecondaryImages)
            throw new AppException($"No se permiten más de {MaxSecondaryImages} imágenes secundarias.", 400);

        if (images.Any(i => string.IsNullOrWhiteSpace(i.Url)))
            throw new AppException("Cada imagen secundaria debe tener una URL.", 400);
    }

    private static void ValidateTags(List<HomeSectionTagDto>? tags)
    {
        if (tags is null) return;

        if (tags.Count > MaxTags)
            throw new AppException($"No se permiten más de {MaxTags} etiquetas.", 400);

        if (tags.Any(t => string.IsNullOrWhiteSpace(t.Label)))
            throw new AppException("Cada etiqueta debe tener un texto.", 400);
    }

    private static string? Serialize<T>(List<T>? items) =>
        items is null ? null : JsonSerializer.Serialize(items);

    private static List<T> Deserialize<T>(string? json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return [];

        try
        {
            return JsonSerializer.Deserialize<List<T>>(json) ?? [];
        }
        catch (JsonException)
        {
            // Un JSON corrupto no debe tumbar el Home: se ignora y se usan los defaults
            // del frontend, que son válidos visualmente.
            return [];
        }
    }

    private static HomeSectionContentDto ToDto(HomeSectionContent s) => new(
        s.Id,
        s.SectionKey,
        s.Enabled,
        s.SectionTitle,
        s.SectionSubtitle,
        s.Eyebrow,
        s.BodyText,
        s.CtaText,
        s.CtaLink,
        s.MainImageUrl,
        s.MainImageAlt,
        s.CardImageUrl,
        Deserialize<HomeSectionImageDto>(s.SecondaryImagesJson),
        Deserialize<HomeSectionTagDto>(s.TagsJson)
    );
}
