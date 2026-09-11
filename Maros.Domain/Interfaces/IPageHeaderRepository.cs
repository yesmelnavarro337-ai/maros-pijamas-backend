using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IPageHeaderRepository
{
    Task<List<PageHeader>> GetAllAsync();
    Task<PageHeader?> GetByKeyAsync(string pageKey);
    Task AddAsync(PageHeader header);
    Task SaveChangesAsync();
}