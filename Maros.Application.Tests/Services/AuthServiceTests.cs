using Maros.Application.DTOs.Auth;
using Maros.Application.Interfaces;
using Maros.Application.Services;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Moq;
using Xunit;

namespace Maros.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenGenerator> _tokenGenerator = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepository.Object, _passwordHasher.Object, _tokenGenerator.Object);
    }

    [Fact]
    public async Task LoginAsync_UsuarioInexistente_DevuelveNull()
    {
        _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(new LoginRequestDto("no-existe@correo.com", "cualquier-clave"));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_UsuarioInactivo_DevuelveNull()
    {
        var user = new User { Email = "inactivo@correo.com", Status = UserStatus.Inactivo };
        _userRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginRequestDto(user.Email, "cualquier-clave"));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ContrasenaIncorrecta_DevuelveNull()
    {
        var user = new User { Email = "admin@correo.com", Status = UserStatus.Activo, PasswordHash = "hash-real" };
        _userRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(user.PasswordHash, "clave-incorrecta")).Returns(false);

        var result = await _sut.LoginAsync(new LoginRequestDto(user.Email, "clave-incorrecta"));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_CredencialesValidas_DevuelveTokenYActualizaUltimoAcceso()
    {
        var user = new User
        {
            Email = "admin@correo.com",
            Name = "Admin",
            Status = UserStatus.Activo,
            Role = UserRole.Administrador,
            PasswordHash = "hash-real",
        };
        _userRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(user.PasswordHash, "clave-correcta")).Returns(true);
        _tokenGenerator.Setup(t => t.GenerateToken(user)).Returns(("token-falso", DateTime.UtcNow.AddHours(1)));

        var result = await _sut.LoginAsync(new LoginRequestDto(user.Email, "clave-correcta"));

        Assert.NotNull(result);
        Assert.Equal("token-falso", result!.Token);
        Assert.Equal("Administrador", result.Role);
        _userRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.NotNull(user.LastAccessAt);
    }
}