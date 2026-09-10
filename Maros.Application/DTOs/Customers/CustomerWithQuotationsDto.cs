namespace Maros.Application.DTOs.Customers;

public record CustomerQuotationSummaryDto(Guid Id, string Status, DateTime CreatedAt, List<string> ProductNames);

public record CustomerWithQuotationsDto(
    Guid Id,
    string Name,
    string Phone,
    string? Email,
    string City,
    DateTime CreatedAt,
    List<CustomerQuotationSummaryDto> Quotations
);