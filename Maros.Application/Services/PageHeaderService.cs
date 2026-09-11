using Maros.Application.DTOs.PageHeaders;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class PageHeaderService : IPageHeaderService
{
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
        header.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(header);
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
        h.OverlayOpacity
    );
}