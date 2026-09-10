using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Settings;

public record SiteSettingsUpdateDto(
    [Required, MaxLength(150)] string SiteName,
    string Description,
    string Currency,
    string Timezone,
    string Language,
    bool MaintenanceMode,
    string? LogoUrl,
    string? Instagram,
    string? Facebook,
    string? TikTok,
    [MaxLength(30)] string WhatsappNumber,
    string WhatsappDefaultMessage,
    string? Address,
    string? BusinessHours,
    string EmailFromName,
    [EmailAddress] string EmailFromAddress,
    bool NotifyNewQuotation,
    string SeoMetaTitle,
    string SeoMetaDescription,
    string? SeoSocialImageUrl,
    string LegalTermsUrl,
    string LegalPrivacyUrl,
    string LegalReturnsPolicy,
    [MaxLength(150)] string CustomDomain,
    bool SslEnabled,
    bool AutoBackupEnabled,
    string BackupFrequency,
    bool TwoFactorEnabled,
    [Range(5, 1440)] int SessionTimeoutMinutes
);