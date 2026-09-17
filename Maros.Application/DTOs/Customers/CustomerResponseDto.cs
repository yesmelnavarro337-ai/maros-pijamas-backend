namespace Maros.Application.DTOs.Customers;

public record CustomerResponseDto(
    Guid Id,
    string Name,
    string Phone,
    string? Email,
    string City,
    DateTime CreatedAt,
    bool IsActive = true,
    int TotalQuotations = 0,
    DateTime? LastActivityAt = null
);

public record CustomerCreateDto(
    string Name,
    string Phone,
    string? Email,
    string City
);

public record CustomerUpdateDto(
    string Name,
    string Phone,
    string? Email,
    string City
);