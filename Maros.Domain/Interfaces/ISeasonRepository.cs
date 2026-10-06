using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ISeasonRepository
{
    Task<List<Season>> GetAllAsync();
    Task<Season?> GetByIdAsync(Guid id);
    Task<Season?> GetByIdReadOnlyAsync(Guid id);
    Task<Season?> GetActiveAsync();
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task DeactivateAllExceptAsync(Guid seasonId);
    Task AddAsync(Season season);
    void Remove(Season season);
    void RemoveImage(SeasonImage image);
    void AddImage(SeasonImage image);
    Task SaveChangesAsync();
}