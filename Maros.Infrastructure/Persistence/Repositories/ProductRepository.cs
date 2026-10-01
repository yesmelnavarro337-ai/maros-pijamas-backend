using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MarosDbContext _context;

    public ProductRepository(MarosDbContext context) => _context = context;

    private IQueryable<Product> QueryWithIncludes() =>
        _context.Products
            .AsSplitQuery()
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.ProductCollections);

    public IQueryable<Product> QueryAll() => QueryWithIncludes();

    public Task<Product?> GetByIdAsync(Guid id) =>
        QueryWithIncludes().FirstOrDefaultAsync(p => p.Id == id);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
        _context.Products.AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId));

    public Task<bool> SkuExistsAsync(string sku, Guid? excludeProductId = null) =>
        _context.ProductVariants.AnyAsync(v =>
            v.Sku.ToLower() == sku.ToLower() && (excludeProductId == null || v.ProductId != excludeProductId));

    public async Task AddAsync(Product product) =>
        await _context.Products.AddAsync(product);

    public void Remove(Product product) =>
        _context.Products.Remove(product);

    public void RemoveVariantsRange(IEnumerable<ProductVariant> variants) =>
        _context.ProductVariants.RemoveRange(variants);

    public void RemoveImagesRange(IEnumerable<ProductImage> images) =>
        _context.ProductImages.RemoveRange(images);

    public void RemoveProductCategoriesRange(IEnumerable<ProductCategory> productCategories) =>
        _context.ProductCategories.RemoveRange(productCategories);

    public void RemoveProductCollectionsRange(IEnumerable<ProductCollection> productCollections) =>
        _context.ProductCollections.RemoveRange(productCollections);

    public void AddVariant(ProductVariant variant) =>
        _context.ProductVariants.Add(variant);

    public void AddImage(ProductImage image) =>
        _context.ProductImages.Add(image);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();

    public Task<Product?> GetBySlugAsync(string slug) =>
        QueryWithIncludes().FirstOrDefaultAsync(p => p.Slug == slug && p.Status == ProductStatus.Activo);

    public Task<List<Product>> GetPublicAsync(Guid? categoryId, Guid? collectionId, string? search)
    {
        var query = QueryWithIncludes().Where(p => p.Status == ProductStatus.Activo);

        if (categoryId.HasValue)
            query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value));

        if (collectionId.HasValue)
            query = query.Where(p => p.ProductCollections.Any(pc => pc.CollectionId == collectionId.Value));

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        return query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public Task<List<Product>> GetFeaturedHomeAsync() =>
        QueryWithIncludes()
            .Where(p => p.Status == ProductStatus.Activo && p.FeaturedHome)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();
}
