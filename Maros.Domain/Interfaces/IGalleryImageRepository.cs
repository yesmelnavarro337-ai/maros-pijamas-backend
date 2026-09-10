using Maros.Domain.Entities;
using Maros.Domain.Enums;

namespace Maros.Domain.Interfaces;

public interface IGalleryImageRepository
{
    IQueryable<GalleryImage> QueryAll();
    Task<List<GalleryImage>> GetActiveAsync(GalleryCategory? category = null);
    Task<GalleryImage?> GetByIdAsync(Guid id);
    Task<int> GetNextOrderAsync(GalleryCategory category);
    Task AddAsync(GalleryImage image);
    void Remove(GalleryImage image);
    Task SaveChangesAsync();
}