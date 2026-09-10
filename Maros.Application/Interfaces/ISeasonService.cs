using Maros.Application.DTOs.Seasons;

namespace Maros.Application.Interfaces;

public interface ISeasonService
{
    Task<List<SeasonResponseDto>> GetAllAsync();
    Task<SeasonResponseDto> GetByIdAsync(Guid id);
    Task<SeasonResponseDto> CreateAsync(SeasonCreateDto request);
    Task<SeasonResponseDto> UpdateAsync(Guid id, SeasonUpdateDto request);
    Task<SeasonResponseDto> ActivateAsync(Guid id);
    Task RemoveAsync(Guid id);
    Task<SeasonPublicResponseDto?> GetActivePublicAsync();
}