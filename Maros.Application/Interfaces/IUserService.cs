using Maros.Application.DTOs.Users;

namespace Maros.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto> InviteAsync(InviteUserRequestDto request);
    Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserRequestDto request);
    Task<UserResponseDto> UpdateRoleAsync(Guid id, UpdateUserRoleRequestDto request, Guid actingUserId);
    Task RemoveAsync(Guid id, Guid actingUserId);
}