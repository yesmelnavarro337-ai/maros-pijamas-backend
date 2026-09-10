using Maros.Application.DTOs.Customization;

namespace Maros.Application.Interfaces;

public interface ICustomizationOptionService
{
    Task<List<CustomizationOptionResponseDto>> GetAllAsync();
    Task<CustomizationOptionResponseDto> CreateAsync(CustomizationOptionCreateDto request);
    Task<CustomizationOptionResponseDto> UpdateAsync(Guid id, CustomizationOptionUpdateDto request);
    Task RemoveAsync(Guid id);
    Task<CustomizationCatalogPublicDto> GetPublicCatalogAsync();
}