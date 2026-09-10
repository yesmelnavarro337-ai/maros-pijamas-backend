namespace Maros.Application.DTOs.Settings;

public record SiteSettingsPublicDto(
    string SiteName,
    string Description,
    bool MaintenanceMode,
    string? LogoUrl,
    string? Instagram,
    string? Facebook,
    string? TikTok,
    string WhatsappNumber,
    string? EmailFromAddress,
    string Address,
    string BusinessHours,
    string SeoMetaTitle,
    string SeoMetaDescription,
    string? SeoSocialImageUrl,
    string LegalTermsUrl,
    string LegalPrivacyUrl
);