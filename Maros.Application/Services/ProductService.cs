using Maros.Application.Common;
using Maros.Application.DTOs.Products;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly IPaginationService _paginationService;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ISeasonRepository seasonRepository,
        IPaginationService paginationService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _seasonRepository = seasonRepository;
        _paginationService = paginationService;
    }

    public async Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQueryParams query)
    {
        var products = _productRepository.QueryAll().Where(p => !p.IsDeleted);

        if (query.CategoryId.HasValue)
            products = products.Where(p => p.CategoryId == query.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            !query.Status.Equals("all", StringComparison.OrdinalIgnoreCase) &&
            !query.Status.Equals("todos", StringComparison.OrdinalIgnoreCase))
        {
            var st = query.Status.ToLowerInvariant();
            if (st is "active" or "activo")
            {
                products = products.Where(p => p.Status == ProductStatus.Activo);
            }
            else if (st is "lowstock" or "stockbajo")
            {
                products = products.Where(p => p.Status == ProductStatus.Activo && p.Variants.Sum(v => v.Stock) > 0 && p.Variants.Sum(v => v.Stock) <= 3);
            }
            else if (st is "outofstock" or "sinstock")
            {
                products = products.Where(p => p.Status == ProductStatus.Activo && p.Variants.Sum(v => v.Stock) == 0);
            }
            else if (Enum.TryParse<ProductStatus>(query.Status, ignoreCase: true, out var enumStatus))
            {
                products = products.Where(p => p.Status == enumStatus);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            products = products.Where(p => p.Name.Contains(search) || p.Slug.Contains(search) || p.Variants.Any(v => v.Sku.Contains(search)));
        }

        if (query.SeasonId.HasValue)
        {
            var season = await _seasonRepository.GetByIdAsync(query.SeasonId.Value);
            if (season != null)
            {
                products = products.Where(p => p.ProductCollections.Any(pc => pc.CollectionId == season.CollectionId));
            }
        }

        products = ApplySorting(products, query.SortBy, query.SortDescending);

        var seasons = await _seasonRepository.GetAllAsync();

        var paged = await _paginationService.PaginateAsync(products, query.PageNumber, query.PageSize);
        var dtos = paged.Items.Select(p => ToDto(p, seasons)).ToList();

        return new PagedResult<ProductResponseDto>(dtos, paged.PageNumber, paged.PageSize, paged.TotalCount);
    }

    public async Task<ProductMetricsDto> GetMetricsAsync()
    {
        var allProducts = _productRepository.QueryAll().Where(p => !p.IsDeleted).ToList();
        var activeProducts = allProducts.Where(p => p.Status == ProductStatus.Activo).ToList();

        var now = DateTime.UtcNow;
        var periodStart = now.AddDays(-30);
        var prevPeriodStart = now.AddDays(-60);

        var currentActiveCount = activeProducts.Count(p => p.CreatedAt >= periodStart);
        var prevActiveCount = activeProducts.Count(p => p.CreatedAt >= prevPeriodStart && p.CreatedAt < periodStart);

        double variationPct = prevActiveCount == 0
            ? (currentActiveCount > 0 ? 100.0 : 0.0)
            : Math.Round(((double)(currentActiveCount - prevActiveCount) / prevActiveCount) * 100.0, 1);

        var lowStockCount = activeProducts.Count(p =>
        {
            var stock = p.Variants.Sum(v => v.Stock);
            return stock > 0 && stock <= 3;
        });

        var outOfStockCount = activeProducts.Count(p => p.Variants.Sum(v => v.Stock) == 0);

        return new ProductMetricsDto(
            ActiveProductsCount: activeProducts.Count,
            ActiveProductsVariationPercentage: variationPct,
            LowStockCount: lowStockCount,
            OutOfStockCount: outOfStockCount
        );
    }

    public async Task<ProductResponseDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new AppException("Producto no encontrado.", 404);
        var seasons = await _seasonRepository.GetAllAsync();
        return ToDto(product, seasons);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto request)
    {
        await ValidateCategoryAsync(request.CategoryId);
        ValidateStatus(request.Status, out var status);
        await ProcessAndValidateVariantSkusAsync(request.Variants, excludeProductId: null);

        var slug = SlugGenerator.Generate(request.Name);
        if (await _productRepository.SlugExistsAsync(slug))
            throw new AppException("Ya existe un producto con un nombre equivalente.", 409);

        var product = new Product
        {
            Name = request.Name,
            Slug = slug,
            CategoryId = request.CategoryId,
            Description = request.Description,
            BasePrice = request.BasePrice,
            Status = status,
            FeaturedHome = request.FeaturedHome,
            AllowCustomization = request.AllowCustomization,
            DeliveryTime = request.DeliveryTime,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            SeoSlug = slug,
            SeoSocialImageUrl = request.SeoSocialImageUrl,
            SeoAltText = request.SeoAltText,
        };

        ApplyImages(product, request.ImageUrls);
        ApplyVariants(product, request.Variants);
        ApplyCollections(product, request.CollectionIds);

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        var created = await _productRepository.GetByIdAsync(product.Id);
        var seasons = await _seasonRepository.GetAllAsync();
        return ToDto(created!, seasons);
    }

    public async Task<ProductResponseDto> UpdateAsync(Guid id, ProductUpdateDto request)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new AppException("Producto no encontrado.", 404);

        await ValidateCategoryAsync(request.CategoryId);
        ValidateStatus(request.Status, out var status);
        await ProcessAndValidateVariantSkusAsync(request.Variants, excludeProductId: id);

        var slug = SlugGenerator.Generate(request.Name);
        if (await _productRepository.SlugExistsAsync(slug, excludeId: id))
            throw new AppException("Ya existe otro producto con un nombre equivalente.", 409);

        product.Name = request.Name;
        product.Slug = slug;
        product.CategoryId = request.CategoryId;
        product.Description = request.Description;
        product.BasePrice = request.BasePrice;
        product.Status = status;
        product.FeaturedHome = request.FeaturedHome;
        product.AllowCustomization = request.AllowCustomization;
        product.DeliveryTime = request.DeliveryTime;
        product.SeoTitle = request.SeoTitle;
        product.SeoDescription = request.SeoDescription;
        product.SeoSlug = slug;
        product.SeoSocialImageUrl = request.SeoSocialImageUrl;
        product.SeoAltText = request.SeoAltText;
        product.UpdatedAt = DateTime.UtcNow;

        product.Images.Clear();
        product.Variants.Clear();
        product.ProductCollections.Clear();

        ApplyImages(product, request.ImageUrls);
        ApplyVariants(product, request.Variants);
        ApplyCollections(product, request.CollectionIds);

        await _productRepository.SaveChangesAsync();

        var updated = await _productRepository.GetByIdAsync(id);
        var seasons = await _seasonRepository.GetAllAsync();
        return ToDto(updated!, seasons);
    }

    public async Task RemoveAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new AppException("Producto no encontrado.", 404);

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _productRepository.SaveChangesAsync();
    }

    public async Task<List<ProductPublicListDto>> GetPublicListAsync(Guid? categoryId, Guid? collectionId, string? search)
    {
        var products = await _productRepository.GetPublicAsync(categoryId, collectionId, search);
        return products.Select(ToPublicListDto).ToList();
    }

    public async Task<ProductPublicDetailDto> GetPublicDetailBySlugAsync(string slug)
    {
        var product = await _productRepository.GetBySlugAsync(slug)
            ?? throw new AppException("Producto no encontrado.", 404);
        return ToPublicDetailDto(product);
    }

    public async Task<List<DTOs.Common.ProductSummaryDto>> GetFeaturedHomeAsync()
    {
        var products = await _productRepository.GetFeaturedHomeAsync();
        return products.Select(p => new DTOs.Common.ProductSummaryDto(
            p.Id, p.Name, p.Slug, p.BasePrice,
            p.Images.OrderBy(i => i.Order).Select(i => i.Url).FirstOrDefault(),
            p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList()
        )).ToList();
    }

    // ─── Helpers privados ──────────────────────────────────────────

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sortBy, bool descending)
    {
        Func<IQueryable<Product>, IOrderedQueryable<Product>> order = sortBy?.ToLowerInvariant() switch
        {
            "name" => descending ? q => q.OrderByDescending(p => p.Name) : q => q.OrderBy(p => p.Name),
            "price" => descending ? q => q.OrderByDescending(p => p.BasePrice) : q => q.OrderBy(p => p.BasePrice),
            _ => descending ? q => q.OrderByDescending(p => p.CreatedAt) : q => q.OrderBy(p => p.CreatedAt),
        };
        return order(query);
    }

    private async Task ValidateCategoryAsync(Guid? categoryId)
    {
        if (categoryId.HasValue && categoryId.Value != Guid.Empty)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId.Value);
            if (category is null)
                throw new AppException("La categoría seleccionada no existe.", 400);
        }
    }

    private static void ValidateStatus(string statusInput, out ProductStatus status)
    {
        if (!Enum.TryParse(statusInput, ignoreCase: true, out status))
            throw new AppException("Estado de producto inválido.", 400);
    }

    private async Task ProcessAndValidateVariantSkusAsync(List<ProductVariantInputDto> variants, Guid? excludeProductId)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < variants.Count; i++)
        {
            var variant = variants[i];
            string sku = variant.Sku;

            if (string.IsNullOrWhiteSpace(sku))
            {
                sku = await GenerateUniqueSkuAsync(variant.Size, variant.ColorName);
                variants[i] = variant with { Sku = sku };
            }
            else
            {
                sku = sku.Trim();
                if (sku != variant.Sku)
                {
                    variants[i] = variant with { Sku = sku };
                }
            }

            if (!seen.Add(sku))
                throw new AppException($"El SKU '{sku}' está repetido dentro del mismo producto. Por favor ingresa o genera uno distinto.", 400);

            if (await _productRepository.SkuExistsAsync(sku))
            {
                var belongsToSameProduct = excludeProductId.HasValue && excludeProductId.Value != Guid.Empty &&
                    (await _productRepository.GetByIdAsync(excludeProductId.Value))?
                        .Variants.Any(v => v.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase)) == true;

                if (!belongsToSameProduct)
                    throw new AppException($"El SKU '{sku}' ya está en uso. Por favor ingresa o genera uno distinto.", 409);
            }
        }
    }

    private async Task<string> GenerateUniqueSkuAsync(string size, string colorName)
    {
        var sizeCode = !string.IsNullOrWhiteSpace(size) ? size.Trim().ToUpperInvariant() : "DEF";
        var colorCode = !string.IsNullOrWhiteSpace(colorName) && colorName.Trim().Length >= 3
            ? colorName.Trim()[..3].ToUpperInvariant()
            : (!string.IsNullOrWhiteSpace(colorName) ? colorName.Trim().ToUpperInvariant() : "VAR");

        string sku;
        do
        {
            var randomSuffix = Random.Shared.Next(1000, 9999);
            sku = $"MP-{sizeCode}-{colorCode}-{randomSuffix}";
        } while (await _productRepository.SkuExistsAsync(sku));

        return sku;
    }

    private static void ApplyImages(Product product, List<string> urls)
    {
        for (int i = 0; i < urls.Count; i++)
            product.Images.Add(new ProductImage { Url = urls[i], Order = i });
    }

    private static void ApplyVariants(Product product, List<ProductVariantInputDto> variants)
    {
        foreach (var v in variants)
        {
            product.Variants.Add(new ProductVariant
            {
                Size = v.Size,
                ColorName = v.ColorName,
                ColorHex = v.ColorHex,
                Sku = v.Sku,
                Stock = v.Stock,
                ImageUrl = v.ImageUrl,
            });
        }
    }

    private static void ApplyCollections(Product product, List<Guid> collectionIds)
    {
        foreach (var collectionId in collectionIds.Distinct())
        {
            product.ProductCollections.Add(new ProductCollection
            {
                ProductId = product.Id,
                CollectionId = collectionId,
            });
        }
    }

    private static ProductPublicListDto ToPublicListDto(Product p) => new(
        p.Id, p.Name, p.Slug, p.Category?.Name ?? string.Empty, p.BasePrice,
        p.Images.OrderBy(i => i.Order).Select(i => i.Url).FirstOrDefault(),
        p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
        p.Variants.Any(v => v.Stock > 0),
        p.Variants.Select(v => v.Size).Distinct().ToList(),
        p.Variants.Select(v => new ProductPublicColorDto(v.ColorName, v.ColorHex)).DistinctBy(c => c.Name).ToList()
    );

    private static ProductPublicDetailDto ToPublicDetailDto(Product p) => new(
        p.Id,
        p.Name,
        p.Slug,
        p.Description,
        p.BasePrice,
        p.Category?.Name ?? string.Empty,
        p.CategoryId,
        p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
        p.Variants.Select(v => v.Size).Distinct().ToList(),
        p.Variants
            .Select(v => new ProductColorPublicDto(v.ColorName, v.ColorHex))
            .DistinctBy(c => c.Name)
            .ToList(),
        p.Variants
            .Select(v => new ProductPublicVariantDto(v.Size, v.ColorName, v.ColorHex, v.Stock > 0))
            .ToList(),
        p.Variants.Any(v => v.Stock > 0),
        p.AllowCustomization,
        p.DeliveryTime,
        p.SeoTitle,
        p.SeoDescription,
        p.SeoSocialImageUrl,
        p.SeoAltText,
        p.ProductCollections.Select(pc => pc.CollectionId).ToList()
    );

    private static ProductResponseDto ToDto(Product p, List<Season>? seasons = null)
    {
        var primarySku = p.Variants.FirstOrDefault()?.Sku ?? (p.Id != Guid.Empty ? $"MP-{p.Id.ToString()[..4].ToUpper()}" : "MP-000");
        var totalStock = p.Variants.Sum(v => v.Stock);
        var imageUrl = p.Images.OrderBy(i => i.Order).Select(i => i.Url).FirstOrDefault();

        string seasonName = "Otoño - Invierno";
        if (seasons != null && p.ProductCollections.Any())
        {
            var collIds = p.ProductCollections.Select(pc => pc.CollectionId).ToHashSet();
            var matchedSeason = seasons.FirstOrDefault(s => collIds.Contains(s.CollectionId));
            if (matchedSeason != null)
            {
                seasonName = matchedSeason.Name;
            }
        }

        return new ProductResponseDto(
            p.Id, p.Name, p.Slug, p.CategoryId, p.Category?.Name ?? "Pijamas de mujer", p.Description,
            p.BasePrice, p.Status.ToString(), p.FeaturedHome, p.AllowCustomization, p.DeliveryTime,
            p.SeoTitle, p.SeoDescription, p.SeoSlug, p.SeoSocialImageUrl, p.SeoAltText,
            p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
            p.Variants.Select(v => new ProductVariantResponseDto(v.Id, v.Size, v.ColorName, v.ColorHex, v.Sku, v.Stock, v.ImageUrl)).ToList(),
            p.ProductCollections.Select(pc => pc.CollectionId).ToList(),
            p.CreatedAt,
            primarySku,
            totalStock,
            seasonName,
            imageUrl
        );
    }
}