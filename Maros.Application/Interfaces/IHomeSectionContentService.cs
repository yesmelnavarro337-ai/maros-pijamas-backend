using Maros.Application.DTOs.Home;

namespace Maros.Application.Interfaces;

public interface IHomeSectionContentService
{
    Task<List<HomeSectionContentDto>> GetAllAsync();
    Task<List<HomeSectionContentDto>> GetPublicAsync();
    Task<HomeSectionContentDto> UpdateAsync(string sectionKey, HomeSectionContentUpdateDto request);
}
