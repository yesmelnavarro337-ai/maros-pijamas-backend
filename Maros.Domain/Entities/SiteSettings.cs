using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class SiteSettings : BaseEntity
{
    // General
    public string SiteName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public bool MaintenanceMode { get; set; }
    public string? LogoUrl { get; set; }

    // Redes sociales
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? TikTok { get; set; }

    // WhatsApp
    public string WhatsappNumber { get; set; } = string.Empty;
    public string WhatsappDefaultMessage { get; set; } = string.Empty;

    // Email
    public string EmailFromName { get; set; } = string.Empty;
    public string EmailFromAddress { get; set; } = string.Empty;
    public bool NotifyNewQuotation { get; set; } = true;

    // SEO global
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;
    public string? SeoSocialImageUrl { get; set; }

    // Legal
    public string LegalTermsUrl { get; set; } = string.Empty;
    public string LegalPrivacyUrl { get; set; } = string.Empty;
    public string LegalReturnsPolicy { get; set; } = string.Empty;

    // Dominio
    public string CustomDomain { get; set; } = string.Empty;
    public bool SslEnabled { get; set; }

    // Backups
    public bool AutoBackupEnabled { get; set; }
    public string BackupFrequency { get; set; } = string.Empty;
    public DateTime? LastBackupDate { get; set; }

    // Seguridad
    public bool TwoFactorEnabled { get; set; }
    public int SessionTimeoutMinutes { get; set; } = 60;
}