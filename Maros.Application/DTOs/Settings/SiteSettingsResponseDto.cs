namespace Maros.Application.DTOs.Settings;

public record SiteSettingsResponseDto(
    string SiteName,
    string Description,
    string Currency,
    string Timezone,
    string Language,
    string DateFormat,
    bool MaintenanceMode,
    string? LogoUrl,
    string? FaviconUrl,
    List<HomeSectionDto> HomeSections,

    // Social
    string? Instagram,
    string? Facebook,
    string? TikTok,
    string? Whatsapp,
    string? Youtube,
    string? Twitter,

    // WhatsApp
    string WhatsappNumber,
    string WhatsappDefaultMessage,
    string? WhatsappButtonImageUrl,
    string WhatsappPosition,
    bool WhatsappButtonEnabled,

    // Contacto
    string ContactPhone,
    string ContactEmail,
    string Address,
    string BusinessHours,
    string? MapImageUrl,
    bool ShowLocation,

    // Email
    string EmailFromName,
    string EmailFromAddress,
    string DefaultSubject,
    string AutoReplyMessage,
    bool NotifyNewQuotation,

    // SEO
    string SeoMetaTitle,
    string SeoMetaDescription,
    string Keywords,
    string CanonicalUrl,
    string RobotsTag,
    string? SeoSocialImageUrl,

    // Legal
    string LegalPrivacyPolicy,
    string LegalTermsAndConditions,
    string LegalCookiesPolicy,
    string LegalTermsUrl,
    string LegalPrivacyUrl,
    string LegalReturnsPolicy,

    // Dominio
    string CustomDomain,
    bool WwwRedirect,
    string ServerIp,
    bool SslEnabled,

    // Backups
    bool AutoBackupEnabled,
    string BackupFrequency,
    string BackupTime,
    string BackupRetentionDays,
    DateTime? LastBackupDate,
    string? LastBackupSize,

    // Seguridad
    bool TwoFactorEnabled,
    int MaxLoginAttempts,
    int LockoutDurationMinutes,
    bool SecurityNotificationsEnabled,
    int SessionTimeoutMinutes
);