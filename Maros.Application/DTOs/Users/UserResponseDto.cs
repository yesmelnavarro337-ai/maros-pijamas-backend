namespace Maros.Application.DTOs.Users;

public record UserResponseDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Status,
    DateTime? LastAccessAt
);