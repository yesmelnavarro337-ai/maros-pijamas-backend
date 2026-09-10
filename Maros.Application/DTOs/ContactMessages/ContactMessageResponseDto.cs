namespace Maros.Application.DTOs.ContactMessages;

public record ContactMessageResponseDto(
    Guid Id,
    string FullName,
    string Phone,
    string? Email,
    string Message,
    bool IsRead,
    DateTime CreatedAt
);