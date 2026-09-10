using Maros.Application.DTOs.Auth;

namespace Maros.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
}