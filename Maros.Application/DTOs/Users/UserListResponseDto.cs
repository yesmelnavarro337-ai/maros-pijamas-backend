namespace Maros.Application.DTOs.Users;

public record UserListResponseDto(
    List<UserResponseDto> Users,
    int TotalCount,
    int ActiveCount,
    int PendingCount
);
