namespace Maros.Application.DTOs.Products;

public record CustomizableProductsPageDto(
    List<ProductPublicListDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    bool HasMore
);