using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IStyleRepository
{
    Task<List<Style>> GetAllAsync();
    Task<List<Style>> GetActiveAsync();
    Task<Style?> GetByIdAsync(Guid id);
    Task<List<Style>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task AddRangeAsync(IEnumerable<Style> styles);
    Task SaveChangesAsync();
}