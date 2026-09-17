namespace Maros.Application.DTOs.Settings;

public record SiteSettingsUpdateDto(
    string? SiteName,
    string? Description,
    string? Currency,
    string? Timezone,
    string? Language,
    string? DateFormat,
    bool? MaintenanceMode,
    string? LogoUrl,
    string? FaviconUrl,
    List<HomeSectionDto>? HomeSections,

    string? Instagram,
    string? Facebook,
    string? TikTok,
    string? Whatsapp,
    string? Youtube,
    string? Twitter,

    string? WhatsappNumber,
    string? WhatsappDefaultMessage,
    string? WhatsappButtonImageUrl,
    string? WhatsappPosition,
    bool? WhatsappButtonEnabled,

    string? ContactPhone,
    string? ContactEmail,
    string? Address,
    string? BusinessHours,
    string? MapImageUrl,
    bool? ShowLocation,

    string? EmailFromName,
    string? EmailFromAddress,
    string? DefaultSubject,
    string? AutoReplyMessage,
    bool? NotifyNewQuotation,

    string? SeoMetaTitle,
    string? SeoMetaDescription,
    string? Keywords,
    string? CanonicalUrl,
    string? RobotsTag,
    string? SeoSocialImageUrl,

    string? LegalPrivacyPolicy,
    string? LegalTermsAndConditions,
    string? LegalCookiesPolicy,
    string? LegalTermsUrl,
    string? LegalPrivacyUrl,
    string? LegalReturnsPolicy,

    string? CustomDomain,
    bool? WwwRedirect,
    string? ServerIp,
    bool? SslEnabled,

    bool? AutoBackupEnabled,
    string? BackupFrequency,
    string? BackupTime,
    string? BackupRetentionDays,
    DateTime? LastBackupDate,
    string? LastBackupSize,

    bool? TwoFactorEnabled,
    int? MaxLoginAttempts,
    int? LockoutDurationMinutes,
    bool? SecurityNotificationsEnabled,
    int? SessionTimeoutMinutes
);