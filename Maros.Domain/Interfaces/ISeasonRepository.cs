using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ISeasonRepository
{
    Task<List<Season>> GetAllAsync();
    Task<Season?> GetByIdAsync(Guid id);
    Task<Season?> GetActiveAsync();
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task DeactivateAllExceptAsync(Guid seasonId);
    Task AddAsync(Season season);
    void Remove(Season season);
    Task SaveChangesAsync();
}