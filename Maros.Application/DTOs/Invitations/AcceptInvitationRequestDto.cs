using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Invitations;

public record AcceptInvitationRequestDto(
    [Required] string Token,
    [Required, MinLength(8), MaxLength(100)] string NewPassword,
    [EmailAddress] string? Email = null
);
