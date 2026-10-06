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
            .Include(s => s.Images)
            .Include(s => s.FeaturedProducts)
                .ThenInclude(fp => fp.Product)
                    .ThenInclude(p => p.Images);

    // Consulta de solo lectura: sin tracking y en varias consultas separadas.
    // `Images` y `FeaturedProducts` son colecciones; sin AsSplitQuery EF las
    // materializa en un único JOIN con explosión cartesiana (n1 x n2 x n3 filas).
    private IQueryable<Season> ReadOnlyQuery() =>
        _context.Seasons
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.Collection)
            .Include(s => s.Images)
            .Include(s => s.FeaturedProducts)
                .ThenInclude(fp => fp.Product)
                    .ThenInclude(p => p.Images);

    public Task<List<Season>> GetAllAsync() =>
        ReadOnlyQuery().OrderByDescending(s => s.StartDate).ToListAsync();

    public Task<Season?> GetByIdAsync(Guid id) =>
        QueryWithIncludes().FirstOrDefaultAsync(s => s.Id == id);

    public Task<Season?> GetByIdReadOnlyAsync(Guid id) =>
        ReadOnlyQuery().FirstOrDefaultAsync(s => s.Id == id);

    public Task<Season?> GetActiveAsync() =>
        ReadOnlyQuery().FirstOrDefaultAsync(s => s.Status == SeasonStatus.Activa);

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

    public void RemoveImage(SeasonImage image) =>
        _context.SeasonImages.Remove(image);

    public void AddImage(SeasonImage image) =>
        _context.SeasonImages.Add(image);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}