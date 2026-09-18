using Maros.Application.Common;
using Maros.Application.DTOs.Invitations;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class InvitationService : IInvitationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public InvitationService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<InvitationSummaryDto> GetSummaryAsync(string token)
    {
        var user = await FindValidPendingUserAsync(token);
        return new InvitationSummaryDto(user.Name, user.Email);
    }

    public async Task AcceptAsync(string token, string newPassword, string? email = null)
    {
        var user = await FindValidPendingUserAsync(token, email);

        user.PasswordHash = _passwordHasher.Hash(newPassword);
        user.Status = UserStatus.Activo;
        user.InviteToken = null;
        user.InviteTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();
    }

    private async Task<User> FindValidPendingUserAsync(string token, string? email = null)
    {
        var user = await _userRepository.GetByInviteTokenAsync(token)
            ?? throw new AppException("La invitación no existe o ya fue utilizada.", 404);

        if (!string.IsNullOrWhiteSpace(email) &&
            !string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            throw new AppException("La invitación no corresponde al correo indicado.", 400);
        }

        if (user.InviteTokenExpiresAt.HasValue && user.InviteTokenExpiresAt.Value < DateTime.UtcNow)
            throw new AppException("La invitación expiró. Solicita una nueva al administrador.", 410);

        if (user.Status != UserStatus.Pendiente)
            throw new AppException("La invitación ya fue utilizada.", 400);

        return user;
    }
}
