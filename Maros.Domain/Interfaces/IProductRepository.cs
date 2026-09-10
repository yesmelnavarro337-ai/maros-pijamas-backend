using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IProductRepository
{
    IQueryable<Product> QueryAll();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<List<Product>> GetPublicAsync(Guid? categoryId, Guid? collectionId, string? search);
    Task<List<Product>> GetFeaturedHomeAsync();
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<bool> SkuExistsAsync(string sku, Guid? excludeVariantId = null);
    Task AddAsync(Product product);
    void Remove(Product product);
    Task SaveChangesAsync();
}