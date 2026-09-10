using Maros.Application.DTOs.Collections;

namespace Maros.Application.Interfaces;

public interface ICollectionService
{
    Task<List<CollectionResponseDto>> GetAllAsync();
    Task<CollectionResponseDto> GetByIdAsync(Guid id);
    Task<CollectionResponseDto> CreateAsync(CollectionCreateDto request);
    Task<CollectionResponseDto> UpdateAsync(Guid id, CollectionUpdateDto request);
    Task<CollectionResponseDto> SetDefaultAsync(Guid id);
    Task RemoveAsync(Guid id);
    Task<CollectionPublicResponseDto> GetActivePublicAsync();
    Task<List<CollectionListPublicDto>> GetAllPublicAsync();
}