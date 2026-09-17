using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class SiteSettingsRepository : ISiteSettingsRepository
{
    private readonly MarosDbContext _context;

    public SiteSettingsRepository(MarosDbContext context) => _context = context;

    public Task<SiteSettings?> GetAsync() =>
        _context.SiteSettings.FirstOrDefaultAsync();

    public async Task AddAsync(SiteSettings settings) =>
        await _context.SiteSettings.AddAsync(settings);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}