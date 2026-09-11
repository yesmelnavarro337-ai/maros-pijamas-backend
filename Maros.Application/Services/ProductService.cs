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
    private readonly IPaginationService _paginationService;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IPaginationService paginationService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _paginationService = paginationService;
    }

    public async Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQueryParams query)
    {
        var products = _productRepository.QueryAll();

        if (query.CategoryId.HasValue)
            products = products.Where(p => p.CategoryId == query.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<ProductStatus>(query.Status, ignoreCase: true, out var status))
            products = products.Where(p => p.Status == status);

        if (!string.IsNullOrWhiteSpace(query.Search))
            products = products.Where(p => p.Name.Contains(query.Search));

        products = ApplySorting(products, query.SortBy, query.SortDescending);

        var paged = await _paginationService.PaginateAsync(products, query.PageNumber, query.PageSize);
        return new PagedResult<ProductResponseDto>(paged.Items.Select(ToDto).ToList(), paged.PageNumber, paged.PageSize, paged.TotalCount);
    }

    public async Task<ProductResponseDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new AppException("Producto no encontrado.", 404);
        return ToDto(product);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto request)
    {
        await ValidateCategoryAsync(request.CategoryId);
        ValidateStatus(request.Status, out var status);
        await ValidateVariantSkusAsync(request.Variants, excludeProductId: null);

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
        return ToDto(created!);
    }

    public async Task<ProductResponseDto> UpdateAsync(Guid id, ProductUpdateDto request)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new AppException("Producto no encontrado.", 404);

        await ValidateCategoryAsync(request.CategoryId);
        ValidateStatus(request.Status, out var status);
        await ValidateVariantSkusAsync(request.Variants, excludeProductId: id);

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
        return ToDto(updated!);
    }

    public async Task RemoveAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new AppException("Producto no encontrado.", 404);

        // Borrado lógico: no removemos el registro físico porque puede estar
        // referenciado por otras entidades (cotizaciones, temporadas, etc.).
        // Solo lo desactivamos para que deje de aparecer en el catálogo.
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

    private async Task ValidateCategoryAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category is null)
            throw new AppException("La categoría seleccionada no existe.", 400);
    }

    private static void ValidateStatus(string statusInput, out ProductStatus status)
    {
        if (!Enum.TryParse(statusInput, ignoreCase: true, out status))
            throw new AppException("Estado de producto inválido.", 400);
    }

    private async Task ValidateVariantSkusAsync(List<ProductVariantInputDto> variants, Guid? excludeProductId)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var variant in variants)
        {
            if (!seen.Add(variant.Sku))
                throw new AppException($"El SKU '{variant.Sku}' está repetido dentro del mismo producto.", 400);

            if (await _productRepository.SkuExistsAsync(variant.Sku))
            {
                var belongsToSameProduct = excludeProductId.HasValue &&
                    (await _productRepository.GetByIdAsync(excludeProductId.Value))?
                        .Variants.Any(v => v.Sku.Equals(variant.Sku, StringComparison.OrdinalIgnoreCase)) == true;

                if (!belongsToSameProduct)
                    throw new AppException($"El SKU '{variant.Sku}' ya está en uso por otro producto.", 409);
            }
        }
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

    private static ProductResponseDto ToDto(Product p) => new(
        p.Id, p.Name, p.Slug, p.CategoryId, p.Category?.Name ?? string.Empty, p.Description,
        p.BasePrice, p.Status.ToString(), p.FeaturedHome, p.AllowCustomization, p.DeliveryTime,
        p.SeoTitle, p.SeoDescription, p.SeoSlug, p.SeoSocialImageUrl, p.SeoAltText,
        p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
        p.Variants.Select(v => new ProductVariantResponseDto(v.Id, v.Size, v.ColorName, v.ColorHex, v.Sku, v.Stock, v.ImageUrl)).ToList(),
        p.ProductCollections.Select(pc => pc.CollectionId).ToList(),
        p.CreatedAt
    );
}