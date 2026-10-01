using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IHomeSectionContentRepository
{
    Task<List<HomeSectionContent>> GetAllAsync();
    Task<HomeSectionContent?> GetBySectionKeyAsync(string sectionKey);
    Task<HomeSectionContent?> GetByIdAsync(Guid id);
    Task AddAsync(HomeSectionContent content);
    Task SaveChangesAsync();
}