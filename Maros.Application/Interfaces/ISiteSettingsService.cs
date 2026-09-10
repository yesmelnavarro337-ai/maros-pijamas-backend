using Maros.Application.DTOs.Settings;

namespace Maros.Application.Interfaces;

public interface ISiteSettingsService
{
    Task<SiteSettingsResponseDto> GetAsync();
    Task<SiteSettingsResponseDto> UpdateAsync(SiteSettingsUpdateDto request);
    Task<SiteSettingsPublicDto> GetPublicAsync();
}