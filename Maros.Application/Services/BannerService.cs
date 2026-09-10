using Maros.Application.Common;
using Maros.Application.DTOs.Banners;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class BannerService : IBannerService
{
    private readonly IBannerRepository _repository;

    public BannerService(IBannerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BannerResponseDto>> GetAllAsync()
    {
        var banners = await _repository.GetAllAsync();
        return banners.Select(ToDto).ToList();
    }

    public async Task<BannerResponseDto> CreateAsync(BannerCreateDto request)
    {
        ValidateDateRange(request.StartDate, request.EndDate);

        var banner = new Banner
        {
            Title = request.Title,
            ImageUrl = request.ImageUrl,
            LinkUrl = request.LinkUrl,
            Position = request.Position,
            Active = request.Active,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            SeasonId = request.SeasonId,
            CollectionId = request.CollectionId,
        };

        await _repository.AddAsync(banner);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdAsync(banner.Id);
        return ToDto(created!);
    }

    public async Task<BannerResponseDto> UpdateAsync(Guid id, BannerUpdateDto request)
    {
        var banner = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Banner no encontrado.", 404);

        ValidateDateRange(request.StartDate, request.EndDate);

        banner.Title = request.Title;
        banner.ImageUrl = request.ImageUrl;
        banner.LinkUrl = request.LinkUrl;
        banner.Position = request.Position;
        banner.Active = request.Active;
        banner.StartDate = request.StartDate;
        banner.EndDate = request.EndDate;
        banner.SeasonId = request.SeasonId;
        banner.CollectionId = request.CollectionId;
        banner.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdAsync(id);
        return ToDto(updated!);
    }

    public async Task RemoveAsync(Guid id)
    {
        var banner = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Banner no encontrado.", 404);

        _repository.Remove(banner);
        await _repository.SaveChangesAsync();
    }

    public async Task<List<BannerPublicDto>> GetPublicAsync(string? position)
    {
        var candidates = await _repository.GetCandidatesForPublicAsync();
        var now = DateTime.UtcNow;

        var visible = candidates.Where(b =>
            b.Active &&
            (b.StartDate == null || now >= b.StartDate) &&
            (b.EndDate == null || now <= b.EndDate) &&
            (b.SeasonId == null || b.Season?.Status == SeasonStatus.Activa)
        );

        if (!string.IsNullOrWhiteSpace(position))
            visible = visible.Where(b => b.Position == position);

        return visible.Select(b => new BannerPublicDto(b.Title, b.ImageUrl, b.LinkUrl, b.Position)).ToList();
    }

    private static void ValidateDateRange(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue && end.Value < start.Value)
            throw new AppException("La fecha de fin debe ser posterior a la fecha de inicio.", 400);
    }

    private static BannerResponseDto ToDto(Banner b) => new(
        b.Id, b.Title, b.ImageUrl, b.LinkUrl, b.Position, b.Active,
        b.StartDate, b.EndDate,
        b.SeasonId, b.Season?.Name,
        b.CollectionId, b.Collection?.Name
    );
}