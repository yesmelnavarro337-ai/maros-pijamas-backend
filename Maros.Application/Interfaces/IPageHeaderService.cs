using Maros.Application.DTOs.PageHeaders;

namespace Maros.Application.Interfaces;

public interface IPageHeaderService
{
    Task<List<PageHeaderDto>> GetAllAsync();
    Task<List<PageHeaderDto>> GetPublicAsync();
    Task<PageHeaderDto> UpdateAsync(string pageKey, PageHeaderUpdateDto request);
}