using System.Security.Cryptography;
using System.Web;
using Maros.Application.Common;
using Maros.Application.DTOs.Users;
using Maros.Application.Interfaces;
using Maros.Application.Options;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Maros.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserService> _logger;
    private readonly IOptions<InvitationOptions> _invitationOptions;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger,
        IOptions<InvitationOptions> invitationOptions,
        IServiceScopeFactory serviceScopeFactory)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _invitationOptions = invitationOptions;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<UserListResponseDto> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var dtos = users.Select(ToDto).ToList();
        var total = dtos.Count;
        var active = dtos.Count(u => string.Equals(u.Status, "Activo", StringComparison.OrdinalIgnoreCase));
        var pending = dtos.Count(u => string.Equals(u.Status, "Pendiente", StringComparison.OrdinalIgnoreCase));

        return new UserListResponseDto(dtos, total, active, pending);
    }

    public async Task<UserResponseDto> InviteAsync(InviteUserRequestDto request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
            throw new AppException("Ya existe un usuario con ese correo electrónico.", 409);

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            throw new AppException("Rol inválido.", 400);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = role,
            Status = UserStatus.Pendiente,
        };

        // Mientras el usuario no acepte la invitación, se guarda un hash de
        // una contraseña temporal aleatoria (que nadie conoce) para conservar
        // la integridad del campo PasswordHash. Al aceptar, se reemplaza por
        // la contraseña que elija el propio usuario.
        var temporaryPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
        user.PasswordHash = _passwordHasher.Hash(temporaryPassword);

        user.InviteToken = GenerateInviteToken();
        user.InviteTokenExpiresAt = DateTime.UtcNow.AddHours(_invitationOptions.Value.ExpiresInHours);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        QueueInvitationEmail(user.Email, user.Name, user.InviteToken);

        return ToDto(user);
    }

    private void QueueInvitationEmail(string email, string name, string? inviteToken)
    {
        if (string.IsNullOrWhiteSpace(inviteToken))
            return;

        var frontendUrl = ResolveFrontendUrl(_invitationOptions.Value);
        var encodedToken = HttpUtility.UrlEncode(inviteToken);
        var encodedEmail = HttpUtility.UrlEncode(email);
        var acceptUrl = $"{frontendUrl}/accept-invitation?token={encodedToken}&email={encodedEmail}";

        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                await emailService.SendInvitationAsync(email, name, acceptUrl);
            }
            catch (Exception ex)
            {
                // El usuario ya quedó creado con estado "Pendiente"; un fallo al
                // enviar el correo no debe convertir la invitación en un error 500.
                _logger.LogWarning(ex, "No se pudo enviar el correo de invitación a {Email}. Enlace: {AcceptUrl}", email, acceptUrl);
            }
        });
    }

    private static string GenerateInviteToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string ResolveFrontendUrl(InvitationOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.FrontendUrl))
            return options.FrontendUrl.TrimEnd('/');

        var acceptUrl = options.AcceptUrl?.Trim();
        if (string.IsNullOrWhiteSpace(acceptUrl))
            return "https://maros-admin.vercel.app";

        const string legacyPath = "/accept-invite";
        const string currentPath = "/accept-invitation";

        return acceptUrl.EndsWith(legacyPath, StringComparison.OrdinalIgnoreCase)
            ? acceptUrl[..^legacyPath.Length].TrimEnd('/')
            : acceptUrl.EndsWith(currentPath, StringComparison.OrdinalIgnoreCase)
                ? acceptUrl[..^currentPath.Length].TrimEnd('/')
                : acceptUrl.TrimEnd('/');
    }

    public async Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new AppException("Usuario no encontrado.", 404);

        if (await _userRepository.EmailExistsAsync(request.Email, excludeUserId: id))
            throw new AppException("Ya existe otro usuario con ese correo electrónico.", 409);

        user.Name = request.Name;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task<UserResponseDto> UpdateRoleAsync(Guid id, UpdateUserRoleRequestDto request, Guid actingUserId)
    {
        if (id == actingUserId)
            throw new AppException("No puedes cambiar tu propio rol.", 403);

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new AppException("Usuario no encontrado.", 404);

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            throw new AppException("Rol inválido.", 400);

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task RemoveAsync(Guid id, Guid actingUserId)
    {
        if (id == actingUserId)
            throw new AppException("No puedes eliminarte a ti mismo del panel.", 403);

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new AppException("Usuario no encontrado.", 404);

        // Borrado lógico: no removemos el registro físico porque puede estar
        // referenciado por otras entidades. Solo lo desactivamos.
        user.Status = UserStatus.Inactivo;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();
    }

    private static UserResponseDto ToDto(User user) => new(
        user.Id,
        user.Name,
        user.Email,
        user.Role.ToString(),
        user.Status.ToString(),
        user.LastAccessAt
    );
}
