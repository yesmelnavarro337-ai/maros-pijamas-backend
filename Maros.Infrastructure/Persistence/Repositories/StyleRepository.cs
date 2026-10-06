using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class StyleRepository : IStyleRepository
{
    private readonly MarosDbContext _context;

    public StyleRepository(MarosDbContext context) => _context = context;

    public Task<List<Style>> GetAllAsync() =>
        _context.Styles
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();

    public Task<List<Style>> GetActiveAsync() =>
        _context.Styles
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();

    public Task<Style?> GetByIdAsync(Guid id) =>
        _context.Styles.FirstOrDefaultAsync(s => s.Id == id);

    public Task<List<Style>> GetByIdsAsync(IEnumerable<Guid> ids) =>
        _context.Styles
            .Where(s => ids.Contains(s.Id))
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();

    public async Task AddRangeAsync(IEnumerable<Style> styles) =>
        await _context.Styles.AddRangeAsync(styles);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}