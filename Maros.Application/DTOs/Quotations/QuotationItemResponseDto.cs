namespace Maros.Application.DTOs.Quotations;

public record QuotationItemOptionDto(Guid Id, string CatalogType, string Name);

public record QuotationItemResponseDto(
    Guid Id,
    Guid? ProductId,
    string ProductName,
    string? ProductImageUrl,
    string Size,
    int Quantity,
    List<QuotationItemOptionDto> SelectedOptions,
    string? EmbroideryText,
    decimal EstimatedUnitPrice
);