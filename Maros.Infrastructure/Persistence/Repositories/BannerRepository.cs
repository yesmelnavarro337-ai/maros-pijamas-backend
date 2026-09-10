using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class BannerRepository : IBannerRepository
{
    private readonly MarosDbContext _context;

    public BannerRepository(MarosDbContext context) => _context = context;

    private IQueryable<Banner> QueryWithIncludes() =>
        _context.Banners.Include(b => b.Season).Include(b => b.Collection);

    public Task<List<Banner>> GetAllAsync() =>
        QueryWithIncludes().OrderBy(b => b.Position).ThenByDescending(b => b.CreatedAt).ToListAsync();

    public Task<Banner?> GetByIdAsync(Guid id) =>
        QueryWithIncludes().FirstOrDefaultAsync(b => b.Id == id);

    public Task<List<Banner>> GetCandidatesForPublicAsync() =>
        // Trae todos los que al menos tienen Active=true; el filtrado fino de
        // fechas y temporada se resuelve en memoria dentro del servicio,
        // porque involucra comparar contra "ahora" y el estado de una
        // entidad relacionada — más claro ahí que como una query SQL compleja.
        QueryWithIncludes().Where(b => b.Active).ToListAsync();

    public async Task AddAsync(Banner banner) =>
        await _context.Banners.AddAsync(banner);

    public void Remove(Banner banner) =>
        _context.Banners.Remove(banner);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}