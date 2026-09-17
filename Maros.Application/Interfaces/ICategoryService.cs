using Maros.Application.DTOs.Categories;

namespace Maros.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto> GetByIdAsync(Guid id);
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto request);
    Task<CategoryResponseDto> UpdateAsync(Guid id, CategoryUpdateDto request);
    Task RemoveAsync(Guid id);

    Task<List<CategoryPublicDto>> GetPublicAsync();
}