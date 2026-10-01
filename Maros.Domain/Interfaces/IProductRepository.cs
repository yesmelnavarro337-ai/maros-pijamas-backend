using Maros.Domain.Entities;
using System.Linq.Expressions;

namespace Maros.Domain.Interfaces;

public interface IProductRepository
{
    IQueryable<Product> QueryAll();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<List<Product>> GetPublicAsync(Guid? categoryId, Guid? collectionId, string? search);
    Task<List<Product>> GetFeaturedHomeAsync();
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<bool> SkuExistsAsync(string sku, Guid? excludeProductId = null);
    Task AddAsync(Product product);
    void Remove(Product product);
    void RemoveVariantsRange(IEnumerable<ProductVariant> variants);
    void RemoveImagesRange(IEnumerable<ProductImage> images);
    void RemoveProductCategoriesRange(IEnumerable<ProductCategory> productCategories);
    void RemoveProductCollectionsRange(IEnumerable<ProductCollection> productCollections);
    void AddVariant(ProductVariant variant);
    void AddImage(ProductImage image);
    Task SaveChangesAsync();
}
