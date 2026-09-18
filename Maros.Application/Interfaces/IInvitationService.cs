using Maros.Application.DTOs.Invitations;

namespace Maros.Application.Interfaces;

public interface IInvitationService
{
    Task<InvitationSummaryDto> GetSummaryAsync(string token);
    Task AcceptAsync(string token, string newPassword, string? email = null);
}
