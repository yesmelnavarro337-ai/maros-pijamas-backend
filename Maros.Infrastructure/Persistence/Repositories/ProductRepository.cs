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
            .Include(p => p.ProductCollections)
            .Include(p => p.ProductStyles)
                .ThenInclude(ps => ps.Style)
            .Include(p => p.CustomizationModel)
                .ThenInclude(m => m!.AssignedOptions);

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

    public void AddVariantsRange(IEnumerable<ProductVariant> variants) =>
        _context.ProductVariants.AddRange(variants);

    /// <summary>
    /// SKUs ya existentes en BD de entre los candidatos dados, en una sola
    /// consulta (WHERE Sku IN …). Permite resolver colisiones en memoria O(N)
    /// sin llamadas ExistsAsync por variante.
    /// </summary>
    public async Task<HashSet<string>> GetExistingSkusAsync(IEnumerable<string> skus, Guid? excludeProductId = null)
    {
        var candidates = skus.ToList();
        if (candidates.Count == 0) return new HashSet<string>(StringComparer.Ordinal);

        var found = await _context.ProductVariants
            .Where(v => candidates.Contains(v.Sku)
                        && (!excludeProductId.HasValue || v.ProductId != excludeProductId.Value))
            .Select(v => v.Sku)
            .ToListAsync();

        return new HashSet<string>(found, StringComparer.Ordinal);
    }

    /// <summary>
    /// Todos los SKUs con un prefijo dado (p. ej. "PIJ-ABCD-"): cubre la base y
    /// cualquier sufijo anticolisión existente en una sola consulta LIKE.
    /// </summary>
    public async Task<HashSet<string>> GetSkusStartingWithAsync(string prefix, Guid? excludeProductId = null)
    {
        var found = await _context.ProductVariants
            .Where(v => v.Sku.StartsWith(prefix)
                        && (!excludeProductId.HasValue || v.ProductId != excludeProductId.Value))
            .Select(v => v.Sku)
            .ToListAsync();

        return new HashSet<string>(found, StringComparer.Ordinal);
    }

    public void AddImage(ProductImage image) =>
        _context.ProductImages.Add(image);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();

    public Task<Product?> GetBySlugAsync(string slug) =>
        QueryWithIncludes().FirstOrDefaultAsync(p => p.Slug == slug && p.Status == ProductStatus.Activo);

    public Task<List<Product>> GetPublicAsync(
        Guid? categoryId,
        Guid? collectionId,
        string? search,
        List<string>? categorySlugs = null,
        List<string>? sizes = null,
        List<string>? colors = null)
    {
        var query = QueryWithIncludes().Where(p => p.Status == ProductStatus.Activo);

        if (categoryId.HasValue)
            query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value));

        if (categorySlugs is { Count: > 0 })
        {
            var slugs = categorySlugs
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLower())
                .Distinct()
                .ToList();
            if (slugs.Count > 0)
                query = query.Where(p => p.ProductCategories.Any(pc => slugs.Contains(pc.Category.Slug.ToLower())));
        }

        if (collectionId.HasValue)
            query = query.Where(p => p.ProductCollections.Any(pc => pc.CollectionId == collectionId.Value));

        if (sizes is { Count: > 0 })
        {
            var sizeValues = sizes
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLower())
                .Distinct()
                .ToList();
            if (sizeValues.Count > 0)
                query = query.Where(p => p.Variants.Any(v => sizeValues.Contains(v.Size.ToLower())));
        }

        if (colors is { Count: > 0 })
        {
            var colorValues = colors
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim().ToLower())
                .Distinct()
                .ToList();
            if (colorValues.Count > 0)
                query = query.Where(p => p.Variants.Any(v => colorValues.Contains(v.ColorName.ToLower())));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Coincidencia global para el buscador de la tienda: título,
            // descripción, categorías, estilos, telas/materiales y colores.
            var term = search.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                p.Description.Contains(term) ||
                p.ProductCategories.Any(pc => pc.Category.Name.Contains(term)) ||
                p.ProductStyles.Any(ps => ps.Style.Name.Contains(term)) ||
                p.Variants.Any(v =>
                    (v.MaterialName != null && v.MaterialName.Contains(term)) ||
                    (v.StyleName != null && v.StyleName.Contains(term)) ||
                    v.ColorName.Contains(term)));
        }

        return query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public Task<List<Product>> GetFeaturedHomeAsync() =>
        QueryWithIncludes()
            .Where(p => p.Status == ProductStatus.Activo && p.FeaturedHome)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

    public Task<List<Product>> GetFeaturedCatalogAsync() =>
        QueryWithIncludes()
            .Where(p => p.IsFeaturedCatalog)
            .OrderBy(p => p.CatalogOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();

    public Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return Task.FromResult(new List<Product>());
        return QueryWithIncludes()
            .Where(p => idList.Contains(p.Id) && !p.IsDeleted)
            .ToListAsync();
    }

    public Task<List<Product>> GetCustomizableAsync(int pageNumber, int pageSize) =>
        QueryWithIncludes()
            .Where(p => !p.IsDeleted && p.Status == ProductStatus.Activo && p.AllowCustomization)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public Task<int> CountCustomizableAsync() =>
        _context.Products.CountAsync(p => !p.IsDeleted && p.Status == ProductStatus.Activo && p.AllowCustomization);
}
