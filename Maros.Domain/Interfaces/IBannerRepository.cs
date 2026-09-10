using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IBannerRepository
{
    Task<List<Banner>> GetAllAsync();
    Task<Banner?> GetByIdAsync(Guid id);
    Task<List<Banner>> GetCandidatesForPublicAsync();
    Task AddAsync(Banner banner);
    void Remove(Banner banner);
    Task SaveChangesAsync();
}