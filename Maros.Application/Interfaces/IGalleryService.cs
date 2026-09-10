using Maros.Application.DTOs.Gallery;
using Maros.Application.Common;
using Maros.Domain.Enums;

namespace Maros.Application.Interfaces;

public interface IGalleryService
{
    Task<PagedResult<GalleryImageResponseDto>> GetAllAsync(GalleryQueryParams query);
    Task<GalleryImageResponseDto> UploadAsync(Stream fileStream, string fileName, GalleryCategory category, string caption);
    Task RemoveAsync(Guid id);
    Task<List<GalleryImagePublicDto>> GetPublicAsync(GalleryCategory? category);
}