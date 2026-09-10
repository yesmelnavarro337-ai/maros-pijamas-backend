using Maros.Application.Common;
using Maros.Application.DTOs.Gallery;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class GalleryService : IGalleryService
{
    private readonly IGalleryImageRepository _repository;
    private readonly IImageStorageService _imageStorage;
    private readonly IPaginationService _paginationService;

    public GalleryService(
        IGalleryImageRepository repository,
        IImageStorageService imageStorage,
        IPaginationService paginationService)
    {
        _repository = repository;
        _imageStorage = imageStorage;
        _paginationService = paginationService;
    }

    public async Task<PagedResult<GalleryImageResponseDto>> GetAllAsync(GalleryQueryParams query)
    {
        var images = _repository.QueryAll();

        if (!string.IsNullOrWhiteSpace(query.Category) &&
            Enum.TryParse<GalleryCategory>(query.Category, ignoreCase: true, out var category))
            images = images.Where(i => i.Category == category);

        images = query.SortBy?.ToLowerInvariant() == "order"
            ? images.OrderBy(i => i.Order)
            : (query.SortDescending ? images.OrderByDescending(i => i.CreatedAt) : images.OrderBy(i => i.CreatedAt));

        var paged = await _paginationService.PaginateAsync(images, query.PageNumber, query.PageSize);
        return new PagedResult<GalleryImageResponseDto>(
    paged.Items.Select(ToDto).ToList(),
    paged.PageNumber,
    paged.PageSize,
    paged.TotalCount);
    }

    public async Task<GalleryImageResponseDto> UploadAsync(Stream fileStream, string fileName, GalleryCategory category, string caption)
    {
        var uploadResult = await _imageStorage.UploadAsync(fileStream, fileName, folder: "gallery");
        var order = await _repository.GetNextOrderAsync(category);

        var image = new GalleryImage
        {
            Url = uploadResult.Url,
            PublicId = uploadResult.PublicId,
            Category = category,
            Caption = caption,
            Order = order,
            Active = true,
        };

        await _repository.AddAsync(image);
        await _repository.SaveChangesAsync();

        return ToDto(image);
    }

    public async Task RemoveAsync(Guid id)
    {
        var image = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Imagen no encontrada.", 404);

        await _imageStorage.DeleteAsync(image.PublicId);

        _repository.Remove(image);
        await _repository.SaveChangesAsync();
    }

    public async Task<List<GalleryImagePublicDto>> GetPublicAsync(GalleryCategory? category)
    {
        var images = await _repository.GetActiveAsync(category);
        return images
            .OrderBy(i => i.Order)
            .Select(i => new GalleryImagePublicDto(i.Url, i.Category.ToString(), i.Caption))
            .ToList();
    }

    private static GalleryImageResponseDto ToDto(GalleryImage i) =>
        new(i.Id, i.Url, i.Category.ToString(), i.Caption, i.Order, i.Active);
}