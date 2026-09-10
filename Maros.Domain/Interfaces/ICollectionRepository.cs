using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ICollectionRepository
{
    Task<List<Collection>> GetAllAsync();
    Task<Collection?> GetByIdAsync(Guid id);
    Task<Collection?> GetActiveViaSeasonAsync();
    Task<Collection?> GetDefaultAsync();
    Task ClearDefaultFlagsAsync();
    Task AddAsync(Collection collection);
    void Remove(Collection collection);
    Task<bool> HasSeasonsAsync(Guid collectionId);
    Task SaveChangesAsync();
}