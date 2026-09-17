using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ISiteSettingsRepository
{
    Task<SiteSettings?> GetAsync();
    Task AddAsync(SiteSettings settings);
    Task SaveChangesAsync();
}