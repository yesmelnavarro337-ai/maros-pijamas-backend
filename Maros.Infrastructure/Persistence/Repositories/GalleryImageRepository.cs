using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class GalleryImageRepository : IGalleryImageRepository
{
    private readonly MarosDbContext _context;

    public GalleryImageRepository(MarosDbContext context) => _context = context;

    public IQueryable<GalleryImage> QueryAll() => _context.GalleryImages;

    public Task<List<GalleryImage>> GetActiveAsync(GalleryCategory? category = null)
    {
        var query = _context.GalleryImages.Where(g => g.Active);
        if (category.HasValue) query = query.Where(g => g.Category == category.Value);
        return query.OrderBy(g => g.Order).ToListAsync();
    }

    public Task<GalleryImage?> GetByIdAsync(Guid id) =>
        _context.GalleryImages.FirstOrDefaultAsync(g => g.Id == id);

    public async Task<int> GetNextOrderAsync(GalleryCategory category)
    {
        var maxOrder = await _context.GalleryImages
            .Where(g => g.Category == category)
            .Select(g => (int?)g.Order)
            .MaxAsync();
        return (maxOrder ?? -1) + 1;
    }

    public async Task AddAsync(GalleryImage image) =>
        await _context.GalleryImages.AddAsync(image);

    public void Remove(GalleryImage image) =>
        _context.GalleryImages.Remove(image);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}