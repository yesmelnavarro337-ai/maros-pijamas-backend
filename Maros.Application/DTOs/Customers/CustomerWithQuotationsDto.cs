namespace Maros.Application.DTOs.Customers;

public record CustomerQuotationSummaryDto(
    Guid Id,
    string Folio,
    string Status,
    DateTime CreatedAt,
    List<string> ProductNames,
    List<string> CustomizationBadges,
    decimal TotalAmount
);

public record CustomerActivityLogDto(
    string Id,
    string Type, // "message", "quotation_sent", "quotation_approved", "registered"
    string Title,
    string Description,
    DateTime Timestamp
);

public record CustomerWithQuotationsDto(
    Guid Id,
    string Name,
    string Phone,
    string? Email,
    string City,
    DateTime CreatedAt,
    bool IsActive,
    int TotalQuotations,
    int TotalMessages,
    DateTime? LastActivityAt,
    List<CustomerQuotationSummaryDto> Quotations,
    List<CustomerActivityLogDto> ActivityLogs
);