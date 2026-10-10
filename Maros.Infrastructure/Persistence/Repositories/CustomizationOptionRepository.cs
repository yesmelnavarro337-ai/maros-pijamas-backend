using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class CustomizationOptionRepository : ICustomizationOptionRepository
{
    private readonly MarosDbContext _context;

    public CustomizationOptionRepository(MarosDbContext context) => _context = context;

    private IQueryable<CustomizationOption> QueryWithIncludes() =>
        _context.CustomizationOptions.Include(o => o.AssignedOptions);

    public Task<List<CustomizationOption>> GetAllAsync() =>
        QueryWithIncludes().OrderBy(o => o.CatalogType).ThenBy(o => o.Name).ToListAsync();

    public Task<List<CustomizationOption>> GetByTypeAsync(CustomizationCatalogType type) =>
        _context.CustomizationOptions.Where(o => o.CatalogType == type).OrderBy(o => o.Name).ToListAsync();

    public Task<List<CustomizationOption>> GetActiveAsync() =>
        _context.CustomizationOptions.Where(o => o.Active).OrderBy(o => o.CatalogType).ThenBy(o => o.Name).ToListAsync();

    public Task<CustomizationOption?> GetByIdAsync(Guid id) =>
        QueryWithIncludes().FirstOrDefaultAsync(o => o.Id == id);

    public Task<List<CustomizationOption>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return Task.FromResult(new List<CustomizationOption>());
        return _context.CustomizationOptions.Where(o => idList.Contains(o.Id)).ToListAsync();
    }

    public async Task AddAsync(CustomizationOption option) =>
        await _context.CustomizationOptions.AddAsync(option);

    public void Remove(CustomizationOption option) =>
        _context.CustomizationOptions.Remove(option);

    public Task<int> DeleteByIdAsync(Guid id) =>
        _context.CustomizationOptions.Where(o => o.Id == id).ExecuteDeleteAsync();

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
