namespace Maros.Application.DTOs.Customers;

public record CustomerResponseDto(
    Guid Id,
    string Name,
    string Phone,
    string? Email,
    string City,
    DateTime CreatedAt
);