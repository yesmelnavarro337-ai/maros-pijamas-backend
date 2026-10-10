using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class SiteSettingsRepository : ISiteSettingsRepository
{
    private const long InitialVisitors = 500;

    private readonly MarosDbContext _context;

    public SiteSettingsRepository(MarosDbContext context) => _context = context;

    public Task<SiteSettings?> GetAsync() =>
        _context.SiteSettings.FirstOrDefaultAsync();

    public async Task AddAsync(SiteSettings settings) =>
        await _context.SiteSettings.AddAsync(settings);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();

    public async Task<long> GetVisitorsCountAsync()
    {
        var count = await _context.SiteSettings
            .Select(s => (long?)s.TotalVisitors)
            .FirstOrDefaultAsync();
        return count ?? InitialVisitors;
    }

    public async Task<long> IncrementVisitorsAsync()
    {
        // Garantiza la fila singleton antes de incrementar.
        if (!await _context.SiteSettings.AnyAsync())
        {
            _context.SiteSettings.Add(new SiteSettings());
            await _context.SaveChangesAsync();
        }

        // Actualización atómica en PostgreSQL (UPDATE ... SET "TotalVisitors"
        // = "TotalVisitors" + 1). Evita condiciones de carrera y actualizaciones
        // perdidas aunque múltiples usuarios entren al mismo tiempo.
        await _context.SiteSettings
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.TotalVisitors, x => x.TotalVisitors + 1));

        return await _context.SiteSettings
            .Select(s => s.TotalVisitors)
            .FirstAsync();
    }
}