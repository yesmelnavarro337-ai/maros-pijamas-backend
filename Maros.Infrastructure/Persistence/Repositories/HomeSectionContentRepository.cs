using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class HomeSectionContentRepository : IHomeSectionContentRepository
{
    private readonly MarosDbContext _context;

    public HomeSectionContentRepository(MarosDbContext context) => _context = context;

    public Task<List<HomeSectionContent>> GetAllAsync() =>
        _context.HomeSectionContents
            .OrderBy(s => s.SectionKey)
            .ToListAsync();

    public Task<HomeSectionContent?> GetBySectionKeyAsync(string sectionKey) =>
        _context.HomeSectionContents
            .FirstOrDefaultAsync(s => s.SectionKey == sectionKey);

    public Task<HomeSectionContent?> GetByIdAsync(Guid id) =>
        _context.HomeSectionContents
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task AddAsync(HomeSectionContent content) =>
        await _context.HomeSectionContents.AddAsync(content);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}