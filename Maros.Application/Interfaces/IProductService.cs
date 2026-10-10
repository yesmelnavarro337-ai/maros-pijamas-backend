using Maros.Application.DTOs.Common;
using Maros.Application.DTOs.Products;
using Maros.Application.Common;

namespace Maros.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQueryParams query);
    Task<ProductMetricsDto> GetMetricsAsync();
    Task<ProductResponseDto> GetByIdAsync(Guid id);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto request);
    Task<ProductResponseDto> UpdateAsync(Guid id, ProductUpdateDto request);
    Task RemoveAsync(Guid id);

    Task<List<ProductPublicListDto>> GetPublicListAsync(Guid? categoryId, Guid? collectionId, string? search, string? categories = null, string? sizes = null, string? colors = null);
    Task<ProductPublicDetailDto> GetPublicDetailBySlugAsync(string slug);
    Task<List<ProductSummaryDto>> GetFeaturedHomeAsync();
    Task<List<ProductPublicListDto>> GetFeaturedCatalogAsync();
    Task<List<ProductFeaturedSelectionDto>> GetFeaturedCatalogSelectionAsync();
    Task<List<ProductFeaturedSelectionDto>> SetFeaturedCatalogAsync(List<Guid> productIds);
    Task<CustomizableProductsPageDto> GetCustomizableAsync(int page, int pageSize);
}