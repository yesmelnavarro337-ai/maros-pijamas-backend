using Maros.Application.Common;
using Maros.Application.DTOs.Categories;
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
            products = products.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == query.CategoryId.Value));

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
        await ValidateCategoriesAsync(request.CategoryIds);
        ValidateStatus(request.Status, out var status);
        await ProcessAndValidateVariantSkusAsync(request.Variants, excludeProductId: null);

        var slug = SlugGenerator.Generate(request.Name);
        if (await _productRepository.SlugExistsAsync(slug))
            throw new AppException("Ya existe un producto con un nombre equivalente.", 409);

        var product = new Product
        {
            Name = request.Name,
            Slug = slug,
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

        ApplyImages(product, request.ImageUrls, request.Images);
        ApplyVariants(product, request.Variants);
        ApplyProductCategories(product, request.CategoryIds, request.CategoryPrices);
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

        await ValidateCategoriesAsync(request.CategoryIds);
        ValidateStatus(request.Status, out var status);
        await ProcessAndValidateVariantSkusAsync(request.Variants, excludeProductId: id);

        var slug = SlugGenerator.Generate(request.Name);
        if (await _productRepository.SlugExistsAsync(slug, excludeId: id))
            throw new AppException("Ya existe otro producto con un nombre equivalente.", 409);

        var now = DateTime.UtcNow;

        product.Name = request.Name;
        product.Slug = slug;
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
        product.UpdatedAt = now;

        SyncImages(product, request.ImageUrls, request.Images, _productRepository, now);
        SyncVariants(product, request.Variants, _productRepository, now);
        SyncProductCategories(product, request.CategoryIds, request.CategoryPrices, _productRepository);
        SyncProductCollections(product, request.CollectionIds, _productRepository);

        try
        {
            await _productRepository.SaveChangesAsync();
        }
        catch (Exception ex) when (ex.GetType().Name.Contains("Concurrency"))
        {
            throw new AppException("No se pudo actualizar el producto debido a un conflicto de concurrencia con los registros de la base de datos.", 409, ex);
        }

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

    private async Task ValidateCategoriesAsync(List<Guid> categoryIds)
    {
        foreach (var categoryId in categoryIds.Where(id => id != Guid.Empty).Distinct())
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
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

            if (await _productRepository.SkuExistsAsync(sku, excludeProductId))
            {
                throw new AppException($"El SKU '{sku}' ya está en uso por otro producto. Por favor ingresa o genera uno distinto.", 409);
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

    private static void ApplyImages(Product product, List<string> urls, List<ProductImageCreateDto>? images = null)
    {
        if (images != null && images.Count > 0)
        {
            for (int i = 0; i < images.Count; i++)
            {
                var img = images[i];
                product.Images.Add(new ProductImage
                {
                    Url = img.Url,
                    Order = img.Order > 0 ? img.Order : i,
                    ColorHex = string.IsNullOrWhiteSpace(img.ColorHex) ? null : img.ColorHex.Trim(),
                    ColorName = string.IsNullOrWhiteSpace(img.ColorName) ? null : img.ColorName.Trim(),
                });
            }
            return;
        }

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
                Price = v.Price,
                ImageUrl = v.ImageUrl,
            });
        }
    }

    private static void ApplyProductCategories(Product product, List<Guid> categoryIds, List<CategoryPriceInputDto>? categoryPrices = null)
    {
        var priceMap = categoryPrices?.ToDictionary(cp => cp.CategoryId, cp => cp);
        foreach (var categoryId in categoryIds.Where(id => id != Guid.Empty).Distinct())
        {
            CategoryPriceInputDto? cp = null;
            priceMap?.TryGetValue(categoryId, out cp);
            product.ProductCategories.Add(new ProductCategory
            {
                ProductId = product.Id,
                CategoryId = categoryId,
                Price = cp?.Price,
                SurchargeReason = string.IsNullOrWhiteSpace(cp?.SurchargeReason) ? null : cp.SurchargeReason.Trim()
            });
        }
    }

    private static void SyncImages(Product product, List<string> requestImageUrls, List<ProductImageUpdateDto>? requestImages, IProductRepository repository, DateTime now)
    {
        var existingDbImageIds = product.Images.Select(img => img.Id).ToHashSet();

        if (requestImages != null && requestImages.Count > 0)
        {
            var validIncomingExistingImageIds = requestImages
                .Where(img => img.Id.HasValue && img.Id.Value != Guid.Empty && existingDbImageIds.Contains(img.Id.Value))
                .Select(img => img.Id!.Value)
                .ToHashSet();

            var imagesToRemove = product.Images
                .Where(img => !validIncomingExistingImageIds.Contains(img.Id))
                .ToList();

            if (imagesToRemove.Count > 0)
            {
                repository.RemoveImagesRange(imagesToRemove);
                foreach (var image in imagesToRemove)
                    product.Images.Remove(image);
            }

            for (int i = 0; i < requestImages.Count; i++)
            {
                var dtoImage = requestImages[i];
                int order = dtoImage.Order > 0 ? dtoImage.Order : i;
                string? colorHex = string.IsNullOrWhiteSpace(dtoImage.ColorHex) ? null : dtoImage.ColorHex.Trim();
                string? colorName = string.IsNullOrWhiteSpace(dtoImage.ColorName) ? null : dtoImage.ColorName.Trim();

                bool isExistingInDb = dtoImage.Id.HasValue && dtoImage.Id.Value != Guid.Empty && existingDbImageIds.Contains(dtoImage.Id.Value);

                if (isExistingInDb)
                {
                    var existing = product.Images.FirstOrDefault(img => img.Id == dtoImage.Id!.Value);
                    if (existing != null)
                    {
                        existing.Url = dtoImage.Url;
                        existing.Order = order;
                        existing.ColorHex = colorHex;
                        existing.ColorName = colorName;
                        existing.UpdatedAt = now;
                    }
                    else
                    {
                        // Fallback: ID was in DTO but entity not found in collection after removal — insert as new
                        repository.AddImage(new ProductImage
                        {
                            ProductId = product.Id,
                            Url = dtoImage.Url,
                            Order = order,
                            ColorHex = colorHex,
                            ColorName = colorName,
                            CreatedAt = now,
                            UpdatedAt = now,
                        });
                    }
                }
                else
                {
                    // New image: use repository.AddImage to force EntityState.Added (INSERT)
                    repository.AddImage(new ProductImage
                    {
                        ProductId = product.Id,
                        Url = dtoImage.Url,
                        Order = order,
                        ColorHex = colorHex,
                        ColorName = colorName,
                        CreatedAt = now,
                        UpdatedAt = now,
                    });
                }
            }
            return;
        }

        var fallbackIncoming = requestImageUrls
            .Select((url, index) => new { Url = url, Order = index })
            .ToList();
        var fallbackUrls = fallbackIncoming
            .Select(i => i.Url)
            .ToHashSet(StringComparer.Ordinal);

        var fallbackToRemove = product.Images
            .Where(img => !fallbackUrls.Contains(img.Url))
            .ToList();

        if (fallbackToRemove.Count > 0)
        {
            repository.RemoveImagesRange(fallbackToRemove);
            foreach (var image in fallbackToRemove)
                product.Images.Remove(image);
        }

        var usedFallbackImageIds = new HashSet<Guid>();

        foreach (var incoming in fallbackIncoming)
        {
            var existing = product.Images.FirstOrDefault(img =>
                img.Url.Equals(incoming.Url, StringComparison.Ordinal) && !usedFallbackImageIds.Contains(img.Id));

            if (existing is not null)
            {
                usedFallbackImageIds.Add(existing.Id);
                if (existing.Order != incoming.Order)
                {
                    existing.Order = incoming.Order;
                    existing.UpdatedAt = now;
                }
                continue;
            }

            // New image via fallback URL path: use repository.AddImage to force INSERT
            repository.AddImage(new ProductImage
            {
                ProductId = product.Id,
                Url = incoming.Url,
                Order = incoming.Order,
                CreatedAt = now,
                UpdatedAt = now,
            });
        }
    }

    private static void SyncVariants(Product product, List<ProductVariantInputDto> requestVariants, IProductRepository repository, DateTime now)
    {
        var existingDbVariantIds = product.Variants.Select(v => v.Id).ToHashSet();

        var validIncomingExistingVariantIds = requestVariants
            .Where(v => v.Id.HasValue && v.Id.Value != Guid.Empty && existingDbVariantIds.Contains(v.Id.Value))
            .Select(v => v.Id!.Value)
            .ToHashSet();

        var variantsToRemove = product.Variants
            .Where(v => !validIncomingExistingVariantIds.Contains(v.Id))
            .ToList();

        if (variantsToRemove.Count > 0)
        {
            repository.RemoveVariantsRange(variantsToRemove);
            foreach (var variant in variantsToRemove)
                product.Variants.Remove(variant);
        }

        foreach (var dtoVariant in requestVariants)
        {
            bool isExistingInDb = dtoVariant.Id.HasValue && dtoVariant.Id.Value != Guid.Empty && existingDbVariantIds.Contains(dtoVariant.Id.Value);

            if (isExistingInDb)
            {
                var existing = product.Variants.FirstOrDefault(v => v.Id == dtoVariant.Id!.Value);
                if (existing != null)
                {
                    existing.Size = dtoVariant.Size;
                    existing.ColorName = dtoVariant.ColorName;
                    existing.ColorHex = dtoVariant.ColorHex;
                    existing.Sku = dtoVariant.Sku;
                    existing.Stock = dtoVariant.Stock;
                    existing.Price = dtoVariant.Price;
                    existing.ImageUrl = dtoVariant.ImageUrl;
                    existing.UpdatedAt = now;
                }
                else
                {
                    // Fallback: ID was in DTO but entity not found in collection — insert as new
                    repository.AddVariant(new ProductVariant
                    {
                        ProductId = product.Id,
                        Size = dtoVariant.Size,
                        ColorName = dtoVariant.ColorName,
                        ColorHex = dtoVariant.ColorHex,
                        Sku = dtoVariant.Sku,
                        Stock = dtoVariant.Stock,
                        Price = dtoVariant.Price,
                        ImageUrl = dtoVariant.ImageUrl,
                        CreatedAt = now,
                        UpdatedAt = now,
                    });
                }
            }
            else
            {
                // New variant: use repository.AddVariant to force EntityState.Added (INSERT)
                repository.AddVariant(new ProductVariant
                {
                    ProductId = product.Id,
                    Size = dtoVariant.Size,
                    ColorName = dtoVariant.ColorName,
                    ColorHex = dtoVariant.ColorHex,
                    Sku = dtoVariant.Sku,
                    Stock = dtoVariant.Stock,
                    Price = dtoVariant.Price,
                    ImageUrl = dtoVariant.ImageUrl,
                    CreatedAt = now,
                    UpdatedAt = now,
                });
            }
        }
    }

    private static void SyncProductCategories(Product product, List<Guid> requestCategoryIds, List<CategoryPriceInputDto>? categoryPrices, IProductRepository repository)
    {
        var incomingCategoryIds = requestCategoryIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToHashSet();

        var priceMap = categoryPrices?.ToDictionary(cp => cp.CategoryId, cp => cp);

        var productCategoriesToRemove = product.ProductCategories
            .Where(pc => !incomingCategoryIds.Contains(pc.CategoryId))
            .ToList();

        if (productCategoriesToRemove.Count > 0)
        {
            repository.RemoveProductCategoriesRange(productCategoriesToRemove);
            foreach (var productCategory in productCategoriesToRemove)
                product.ProductCategories.Remove(productCategory);
        }

        foreach (var categoryId in incomingCategoryIds)
        {
            CategoryPriceInputDto? cp = null;
            priceMap?.TryGetValue(categoryId, out cp);
            var existing = product.ProductCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
            if (existing is not null)
            {
                existing.Price = cp?.Price;
                existing.SurchargeReason = string.IsNullOrWhiteSpace(cp?.SurchargeReason) ? null : cp.SurchargeReason.Trim();
                continue;
            }

            product.ProductCategories.Add(new ProductCategory
            {
                ProductId = product.Id,
                CategoryId = categoryId,
                Price = cp?.Price,
                SurchargeReason = string.IsNullOrWhiteSpace(cp?.SurchargeReason) ? null : cp.SurchargeReason.Trim()
            });
        }
    }

    private static void SyncProductCollections(Product product, List<Guid> requestCollectionIds, IProductRepository repository)
    {
        var incomingCollectionIds = requestCollectionIds
            .Distinct()
            .ToHashSet();

        var productCollectionsToRemove = product.ProductCollections
            .Where(pc => !incomingCollectionIds.Contains(pc.CollectionId))
            .ToList();

        if (productCollectionsToRemove.Count > 0)
        {
            repository.RemoveProductCollectionsRange(productCollectionsToRemove);
            foreach (var productCollection in productCollectionsToRemove)
                product.ProductCollections.Remove(productCollection);
        }

        foreach (var collectionId in incomingCollectionIds)
        {
            if (product.ProductCollections.Any(pc => pc.CollectionId == collectionId))
                continue;

            product.ProductCollections.Add(new ProductCollection
            {
                ProductId = product.Id,
                CollectionId = collectionId,
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

    private static ProductPublicListDto ToPublicListDto(Product p)
    {
        var categories = GetCategorySummaries(p);

        return new ProductPublicListDto(
            p.Id,
            p.Name,
            p.Slug,
            GetCategoryName(categories),
            categories.Select(c => c.Id).ToList(),
            categories,
            p.BasePrice,
            p.Images.OrderBy(i => i.Order).Select(i => i.Url).FirstOrDefault(),
            p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
            p.Variants.Any(v => v.Stock > 0),
            p.Variants.Select(v => v.Size).Distinct().ToList(),
            p.Variants.Select(v => new ProductPublicColorDto(v.ColorName, v.ColorHex)).DistinctBy(c => c.Name).ToList()
        );
    }

    private static ProductPublicDetailDto ToPublicDetailDto(Product p)
    {
        var categories = GetCategorySummaries(p);
        var imageDetails = p.Images
            .OrderBy(i => i.Order)
            .Select(i => new ProductImageDto(i.Id, i.Url, i.Order, i.ColorHex, i.ColorName))
            .ToList();

        return new ProductPublicDetailDto(
            p.Id,
            p.Name,
            p.Slug,
            p.Description,
            p.BasePrice,
            GetCategoryName(categories),
            categories.Select(c => (Guid?)c.Id).FirstOrDefault(),
            categories.Select(c => c.Id).ToList(),
            categories,
            p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
            p.Variants.Select(v => v.Size).Distinct().ToList(),
            p.Variants
                .Select(v => new ProductColorPublicDto(v.ColorName, v.ColorHex))
                .DistinctBy(c => c.Name)
                .ToList(),
            p.Variants
                .Select(v => new ProductPublicVariantDto(v.Size, v.ColorName, v.ColorHex, v.Stock > 0, v.Stock, v.Price))
                .ToList(),
            p.Variants.Any(v => v.Stock > 0),
            p.AllowCustomization,
            p.DeliveryTime,
            p.SeoTitle,
            p.SeoDescription,
            p.SeoSocialImageUrl,
            p.SeoAltText,
            p.ProductCollections.Select(pc => pc.CollectionId).ToList(),
            imageDetails
        );
    }

    private static ProductResponseDto ToDto(Product p, List<Season>? seasons = null)
    {
        var primarySku = p.Variants.FirstOrDefault()?.Sku ?? (p.Id != Guid.Empty ? $"MP-{p.Id.ToString()[..4].ToUpper()}" : "MP-000");
        var totalStock = p.Variants.Sum(v => v.Stock);
        var imageUrl = p.Images.OrderBy(i => i.Order).Select(i => i.Url).FirstOrDefault();
        var categories = GetCategorySummaries(p);

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

        var imageDetails = p.Images
            .OrderBy(i => i.Order)
            .Select(i => new ProductImageDto(i.Id, i.Url, i.Order, i.ColorHex, i.ColorName))
            .ToList();

        return new ProductResponseDto(
            p.Id,
            p.Name,
            p.Slug,
            categories.Select(c => (Guid?)c.Id).FirstOrDefault(),
            GetCategoryName(categories),
            categories.Select(c => c.Id).ToList(),
            categories,
            p.Description,
            p.BasePrice, p.Status.ToString(), p.FeaturedHome, p.AllowCustomization, p.DeliveryTime,
            p.SeoTitle, p.SeoDescription, p.SeoSlug, p.SeoSocialImageUrl, p.SeoAltText,
            p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
            p.Variants.Select(v => new ProductVariantResponseDto(v.Id, v.Size, v.ColorName, v.ColorHex, v.Sku, v.Stock, v.Price, v.ImageUrl)).ToList(),
            p.ProductCollections.Select(pc => pc.CollectionId).ToList(),
            p.CreatedAt,
            primarySku,
            totalStock,
            seasonName,
            imageUrl,
            imageDetails
        );
    }

    private static List<CategorySummaryDto> GetCategorySummaries(Product product) =>
        product.ProductCategories
            .Where(pc => pc.Category is not null)
            .Select(pc => new CategorySummaryDto(
                pc.CategoryId,
                pc.Category.Name,
                pc.Category.Slug,
                pc.Category.DefaultPrice,
                pc.Category.SurchargeReason,
                pc.Price,
                pc.SurchargeReason
            ))
            .DistinctBy(c => c.Id)
            .OrderBy(c => c.Name)
            .ToList();

    private static string GetCategoryName(List<CategorySummaryDto> categories) =>
        categories.Count == 0
            ? "Pijamas de mujer"
            : string.Join(", ", categories.Select(c => c.Name));
}
