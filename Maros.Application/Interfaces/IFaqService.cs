using Maros.Application.DTOs.Faq;

namespace Maros.Application.Interfaces;

public interface IFaqService
{
    Task<List<FaqResponseDto>> GetAllAsync();
    Task<FaqResponseDto> CreateAsync(FaqCreateDto request);
    Task<FaqResponseDto> UpdateAsync(Guid id, FaqUpdateDto request);
    Task ReorderAsync(Guid id, string direction);
    Task RemoveAsync(Guid id);
    Task<List<FaqPublicDto>> GetPublicAsync();
}