using System.Text.Json;
using Maros.Application.DTOs.PageHeaders;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class PageHeaderService : IPageHeaderService
{
    private static readonly JsonSerializerOptions MediaJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IPageHeaderRepository _repository;

    public PageHeaderService(IPageHeaderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PageHeaderDto>> GetAllAsync()
    {
        var headers = await _repository.GetAllAsync();
        return headers.Select(ToDto).ToList();
    }

    public async Task<List<PageHeaderDto>> GetPublicAsync() => await GetAllAsync();

    public async Task<PageHeaderDto> UpdateAsync(string pageKey, PageHeaderUpdateDto request)
    {
        var header = await _repository.GetByKeyAsync(pageKey);

        if (header is null)
        {
            header = new PageHeader { PageKey = pageKey };
            await _repository.AddAsync(header);
        }

        header.Title = request.Title;
        header.Subtitle = request.Subtitle;
        header.BackgroundImageUrl = request.BackgroundImageUrl;
        header.PrimaryButtonText = request.PrimaryButtonText;
        header.PrimaryButtonLink = request.PrimaryButtonLink;
        header.SecondaryButtonText = request.SecondaryButtonText;
        header.SecondaryButtonLink = request.SecondaryButtonLink;
        header.TextColor = request.TextColor;
        header.OverlayOpacity = request.OverlayOpacity;
        header.MediaJson = SerializeMedia(request.Media);
        header.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(header);
    }

    private static string? SerializeMedia(List<PageHeaderMediaDto>? media)
    {
        var normalized = NormalizeMedia(media);
        return normalized.Count == 0 ? null : JsonSerializer.Serialize(normalized, MediaJsonOptions);
    }

    private static List<PageHeaderMediaDto> NormalizeMedia(List<PageHeaderMediaDto>? media)
    {
        if (media is null || media.Count == 0) return [];

        return media
            .Where(m => !string.IsNullOrWhiteSpace(m.Url))
            .Select((m, index) => new PageHeaderMediaDto(
                m.Url.Trim(),
                NormalizeMediaType(m.MediaType),
                index))
            .ToList();
    }

    private static string NormalizeMediaType(string? mediaType)
    {
        var normalized = mediaType?.Trim().ToLowerInvariant();
        return normalized == "video" ? "video" : "image";
    }

    private static List<PageHeaderMediaDto> DeserializeMedia(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try
        {
            var media = JsonSerializer.Deserialize<List<PageHeaderMediaDto>>(json, MediaJsonOptions);
            return media is null
                ? []
                : media
                    .Where(m => !string.IsNullOrWhiteSpace(m.Url))
                    .OrderBy(m => m.Order)
                    .ToList();
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static PageHeaderDto ToDto(PageHeader h) => new(
        h.PageKey,
        h.Title,
        h.Subtitle,
        h.BackgroundImageUrl,
        h.PrimaryButtonText,
        h.PrimaryButtonLink,
        h.SecondaryButtonText,
        h.SecondaryButtonLink,
        h.TextColor,
        h.OverlayOpacity,
        DeserializeMedia(h.MediaJson)
    );
}
