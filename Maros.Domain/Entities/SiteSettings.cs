using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class SiteSettings : BaseEntity
{
    // General
    public string SiteName { get; set; } = "Maros Pijamas";
    public string Description { get; set; } = "Tienda oficial de pijamas y ropa de descanso";
    public string Currency { get; set; } = "COP ($)";
    public string Timezone { get; set; } = "America/Bogota (UTC-5)";
    public string Language { get; set; } = "Español (Colombia)";
    public string DateFormat { get; set; } = "DD/MM/YYYY";
    public bool MaintenanceMode { get; set; }
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }

    // Apariencia — visibilidad/orden de las secciones del home (JSON)
    public string? HomeSectionsJson { get; set; }

    // Redes sociales
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? TikTok { get; set; }
    public string? Whatsapp { get; set; }
    public string? Youtube { get; set; }
    public string? Twitter { get; set; }

    // WhatsApp
    public string WhatsappNumber { get; set; } = "+573013169974";
    public string WhatsappDefaultMessage { get; set; } = "¡Hola! Me gustaría cotizar pijamas al por mayor.";
    public string? WhatsappButtonImageUrl { get; set; }
    public string WhatsappPosition { get; set; } = "right";
    public bool WhatsappButtonEnabled { get; set; } = true;

    // Contacto
    public string ContactPhone { get; set; } = "+57 301 316 9974";
    public string ContactEmail { get; set; } = "contacto@marospijamas.com";
    public string Address { get; set; } = "Calle 10 # 43-12, Medellín, Colombia";
    public string BusinessHours { get; set; } = "Lun - Vie: 8:00 AM - 6:00 PM";
    public string? MapImageUrl { get; set; }
    public bool ShowLocation { get; set; } = true;

    // Email
    public string EmailFromName { get; set; } = "Maros Pijamas";
    public string EmailFromAddress { get; set; } = "ventas@marospijamas.com";
    public string DefaultSubject { get; set; } = "Confirmación de solicitud de cotización";
    public string AutoReplyMessage { get; set; } = "Gracias por escribirnos. Procesaremos tu solicitud en breve.";
    public bool NotifyNewQuotation { get; set; } = true;

    // SEO global
    public string SeoMetaTitle { get; set; } = "Maros Pijamas | Pijamas al por Mayor y Detal";
    public string SeoMetaDescription { get; set; } = "Fabricantes de pijamas en Colombia. Diseños exclusivos en satén y algodón.";
    public string Keywords { get; set; } = "pijamas, moda, satén, ropa de descanso, medellín";
    public string CanonicalUrl { get; set; } = "https://marospijamas.com";
    public string RobotsTag { get; set; } = "index, follow";
    public string? SeoSocialImageUrl { get; set; }

    // Legal
    public string LegalPrivacyPolicy { get; set; } = "Aviso de Privacidad y Tratamiento de Datos Personales...";
    public string LegalTermsAndConditions { get; set; } = "Términos y Condiciones de Uso del sitio web...";
    public string LegalCookiesPolicy { get; set; } = "Política de uso de cookies y almacenamiento local...";
    public string LegalTermsUrl { get; set; } = "/terminos";
    public string LegalPrivacyUrl { get; set; } = "/privacidad";
    public string LegalReturnsPolicy { get; set; } = "/devoluciones";

    // Dominio
    public string CustomDomain { get; set; } = "marospijamas.com";
    public bool WwwRedirect { get; set; } = true;
    public string ServerIp { get; set; } = "185.199.108.153";
    public bool SslEnabled { get; set; } = true;

    // Backups
    public bool AutoBackupEnabled { get; set; } = true;
    public string BackupFrequency { get; set; } = "diaria";
    public string BackupTime { get; set; } = "02:00 AM";
    public string BackupRetentionDays { get; set; } = "30 días";
    public DateTime? LastBackupDate { get; set; }
    public string? LastBackupSize { get; set; } = "24.5 MB";

    // Seguridad
    public bool TwoFactorEnabled { get; set; } = false;
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 15;
    public bool SecurityNotificationsEnabled { get; set; } = true;
    public int SessionTimeoutMinutes { get; set; } = 60;
}