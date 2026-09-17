using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Pendiente;
    public string Phone { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool EmailNotificationsEnabled { get; set; } = true;
    public bool InAppNotificationsEnabled { get; set; } = true;
    public DateTime? LastAccessAt { get; set; }
    public string? InviteToken { get; set; }
    public DateTime? InviteTokenExpiresAt { get; set; }
}