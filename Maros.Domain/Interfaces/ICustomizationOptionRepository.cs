using Maros.Domain.Entities;
using Maros.Domain.Enums;

namespace Maros.Domain.Interfaces;

public interface ICustomizationOptionRepository
{
    Task<List<CustomizationOption>> GetAllAsync();
    Task<List<CustomizationOption>> GetByTypeAsync(CustomizationCatalogType type);
    Task<List<CustomizationOption>> GetActiveAsync();
    Task<CustomizationOption?> GetByIdAsync(Guid id);
    Task AddAsync(CustomizationOption option);
    void Remove(CustomizationOption option);
    Task<int> DeleteByIdAsync(Guid id);
    Task SaveChangesAsync();
}
