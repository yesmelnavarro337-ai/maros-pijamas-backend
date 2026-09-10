namespace Maros.Application.DTOs.Auth;

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    Guid UserId,
    string Name,
    string Email,
    string Role
);