using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly MarosDbContext _context;

    public SeasonRepository(MarosDbContext context) => _context = context;

    private IQueryable<Season> QueryWithIncludes() =>
        _context.Seasons
            .Include(s => s.Collection)
            .Include(s => s.FeaturedProducts)
                .ThenInclude(fp => fp.Product)
                    .ThenInclude(p => p.Images);

    public Task<List<Season>> GetAllAsync() =>
        QueryWithIncludes().OrderByDescending(s => s.StartDate).ToListAsync();

    public Task<Season?> GetByIdAsync(Guid id) =>
        QueryWithIncludes().FirstOrDefaultAsync(s => s.Id == id);

    public Task<Season?> GetActiveAsync() =>
        QueryWithIncludes().FirstOrDefaultAsync(s => s.Status == SeasonStatus.Activa);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
        _context.Seasons.AnyAsync(s => s.Slug == slug && (excludeId == null || s.Id != excludeId));

    public async Task DeactivateAllExceptAsync(Guid seasonId)
    {
        await _context.Seasons
            .Where(s => s.Status == SeasonStatus.Activa && s.Id != seasonId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Status, SeasonStatus.Finalizada));
    }

    public async Task AddAsync(Season season) =>
        await _context.Seasons.AddAsync(season);

    public void Remove(Season season) =>
        _context.Seasons.Remove(season);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}