using Maros.Application.DTOs.Blog;
using Maros.Application.Common;

namespace Maros.Application.Interfaces;

public interface IBlogService
{
    Task<PagedResult<BlogPostResponseDto>> GetAllAsync(BlogQueryParams query);
    Task<BlogPostResponseDto> CreateAsync(BlogPostCreateDto request);
    Task<BlogPostResponseDto> UpdateAsync(Guid id, BlogPostUpdateDto request);
    Task RemoveAsync(Guid id);
    Task<List<BlogPostPublicDto>> GetPublicAsync();
}