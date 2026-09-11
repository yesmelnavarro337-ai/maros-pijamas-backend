using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class PageHeaderRepository : IPageHeaderRepository
{
    private readonly MarosDbContext _context;

    public PageHeaderRepository(MarosDbContext context) => _context = context;

    public Task<List<PageHeader>> GetAllAsync() =>
        _context.PageHeaders.OrderBy(h => h.PageKey).ToListAsync();

    public Task<PageHeader?> GetByKeyAsync(string pageKey) =>
        _context.PageHeaders.FirstOrDefaultAsync(h => h.PageKey == pageKey);

    public async Task AddAsync(PageHeader header) =>
        await _context.PageHeaders.AddAsync(header);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}