using Maros.Application.DTOs.Auth;
using Maros.Application.Interfaces;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || user.Status != UserStatus.Activo)
            return null;

        var isValidPassword = _passwordHasher.Verify(user.PasswordHash, request.Password);
        if (!isValidPassword)
            return null;

        user.LastAccessAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto(
            token,
            expiresAt,
            user.Id,
            user.Name,
            user.Email,
            user.Role.ToString()
        );
    }
}