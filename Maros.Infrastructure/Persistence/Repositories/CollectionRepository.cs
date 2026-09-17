using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class CollectionRepository : ICollectionRepository
{
    private readonly MarosDbContext _context;

    public CollectionRepository(MarosDbContext context) => _context = context;

    private IQueryable<Collection> QueryWithIncludes() =>
        _context.Collections
            .Include(c => c.Seasons)
            .Include(c => c.ProductCollections)
                .ThenInclude(pc => pc.Product)
                    .ThenInclude(p => p.Images);

    public Task<List<Collection>> GetAllAsync() =>
        QueryWithIncludes().OrderBy(c => c.Name).ToListAsync();

    public Task<Collection?> GetByIdAsync(Guid id) =>
        QueryWithIncludes().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Collection?> GetActiveViaSeasonAsync()
    {
        // Consulta mínima de solo lectura sobre Seasons — el CRUD completo
        // de Temporadas (crear, editar, activar) se construye en Fase 8.
        var activeSeason = await _context.Seasons
            .Where(s => s.Status == SeasonStatus.Activa)
            .Select(s => s.CollectionId)
            .FirstOrDefaultAsync();

        if (activeSeason == Guid.Empty) return null;

        return await QueryWithIncludes().FirstOrDefaultAsync(c => c.Id == activeSeason);
    }

    public Task<Collection?> GetDefaultAsync() =>
        QueryWithIncludes().FirstOrDefaultAsync(c => c.IsDefault);

    public async Task ClearDefaultFlagsAsync()
    {
        await _context.Collections
            .Where(c => c.IsDefault)
            .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.IsDefault, false));
    }

    public async Task AddAsync(Collection collection) =>
        await _context.Collections.AddAsync(collection);

    public void Remove(Collection collection) =>
        _context.Collections.Remove(collection);

    public Task<bool> HasSeasonsAsync(Guid collectionId) =>
        _context.Seasons.AnyAsync(s => s.CollectionId == collectionId);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}