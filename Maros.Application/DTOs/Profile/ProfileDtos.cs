using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Profile;

public record UserProfileDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Status,
    DateTime CreatedAt,
    string Phone,
    string? AvatarUrl,
    bool EmailNotificationsEnabled,
    bool InAppNotificationsEnabled
);

public record UpdateProfileRequestDto(
    [Required, MaxLength(100)] string Name,
    [MaxLength(30)] string? Phone = null,
    string? AvatarUrl = null
);

public record RequestEmailChangeDto(
    [Required, EmailAddress, MaxLength(256)] string NewEmail
);

public record VerifyEmailChangeDto(
    [Required, StringLength(6, MinimumLength = 6)] string CodeCurrentEmail,
    [Required, StringLength(6, MinimumLength = 6)] string CodeNewEmail
);

public record ChangePasswordRequestDto(
    [Required] string CurrentPassword,
    [Required, MinLength(6)] string NewPassword,
    [Required, MinLength(6)] string ConfirmPassword
);

public record UpdatePreferencesRequestDto(
    bool EmailNotificationsEnabled,
    bool InAppNotificationsEnabled
);

public record UserAuditLogDto(
    Guid Id,
    string DeviceType,
    string UserAgent,
    string IpAddress,
    DateTime CreatedAt
);
