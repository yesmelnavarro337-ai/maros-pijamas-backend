using Maros.Application.DTOs.Banners;

namespace Maros.Application.Interfaces;

public interface IBannerService
{
    Task<List<BannerResponseDto>> GetAllAsync();
    Task<BannerResponseDto> CreateAsync(BannerCreateDto request);
    Task<BannerResponseDto> UpdateAsync(Guid id, BannerUpdateDto request);
    Task RemoveAsync(Guid id);
    Task<List<BannerPublicDto>> GetPublicAsync(string? position);
}