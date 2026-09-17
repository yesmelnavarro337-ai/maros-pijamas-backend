namespace Maros.Application.DTOs.Quotations;

public record QuotationResponseDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    string CustomerPhone,
    string CustomerCity,
    string Status,
    string Notes,
    List<QuotationItemResponseDto> Items,
    List<string> ReferenceImages,
    DateTime CreatedAt,
    string? CustomerEmail = null,
    DateTime? UpdatedAt = null
);
