using Maros.Application.Common;
using Maros.Application.DTOs.Settings;
using Maros.Application.Interfaces;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class SiteSettingsService : ISiteSettingsService
{
    private readonly ISiteSettingsRepository _repository;

    public SiteSettingsService(ISiteSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<SiteSettingsResponseDto> GetAsync()
    {
        var settings = await _repository.GetAsync()
            ?? throw new AppException("La configuración del sitio aún no ha sido inicializada.", 404);
        return ToDto(settings);
    }

    public async Task<SiteSettingsResponseDto> UpdateAsync(SiteSettingsUpdateDto request)
    {
        var settings = await _repository.GetAsync()
            ?? throw new AppException("La configuración del sitio aún no ha sido inicializada.", 404);

        settings.SiteName = request.SiteName;
        settings.Description = request.Description;
        settings.Currency = request.Currency;
        settings.Timezone = request.Timezone;
        settings.Language = request.Language;
        settings.MaintenanceMode = request.MaintenanceMode;
        settings.LogoUrl = request.LogoUrl;

        settings.Instagram = request.Instagram;
        settings.Facebook = request.Facebook;
        settings.TikTok = request.TikTok;

        settings.WhatsappNumber = request.WhatsappNumber;
        settings.WhatsappDefaultMessage = request.WhatsappDefaultMessage;

        settings.EmailFromName = request.EmailFromName;
        settings.EmailFromAddress = request.EmailFromAddress;
        settings.NotifyNewQuotation = request.NotifyNewQuotation;

        settings.SeoMetaTitle = request.SeoMetaTitle;
        settings.SeoMetaDescription = request.SeoMetaDescription;
        settings.SeoSocialImageUrl = request.SeoSocialImageUrl;

        settings.LegalTermsUrl = request.LegalTermsUrl;
        settings.LegalPrivacyUrl = request.LegalPrivacyUrl;
        settings.LegalReturnsPolicy = request.LegalReturnsPolicy;

        settings.CustomDomain = request.CustomDomain;
        settings.SslEnabled = request.SslEnabled;

        settings.AutoBackupEnabled = request.AutoBackupEnabled;
        settings.BackupFrequency = request.BackupFrequency;
        // LastBackupDate NO se toca aquí — lo actualiza el proceso de backup real, no este endpoint.

        settings.TwoFactorEnabled = request.TwoFactorEnabled;
        settings.SessionTimeoutMinutes = request.SessionTimeoutMinutes;

        settings.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(settings);
    }

    public async Task<SiteSettingsPublicDto> GetPublicAsync()
    {
        var settings = await _repository.GetAsync()
            ?? throw new AppException("La configuración del sitio aún no ha sido inicializada.", 404);

        return new SiteSettingsPublicDto(
            settings.SiteName,
            settings.Description,
            settings.MaintenanceMode,
            settings.LogoUrl,
            settings.Instagram,
            settings.Facebook,
            settings.TikTok,
            settings.WhatsappNumber,
            settings.SeoMetaTitle,
            settings.SeoMetaDescription,
            settings.SeoSocialImageUrl,
            settings.LegalTermsUrl,
            settings.LegalPrivacyUrl
        );
    }

    private static SiteSettingsResponseDto ToDto(Domain.Entities.SiteSettings s) => new(
        s.SiteName, s.Description, s.Currency, s.Timezone, s.Language, s.MaintenanceMode, s.LogoUrl,
        s.Instagram, s.Facebook, s.TikTok,
        s.WhatsappNumber, s.WhatsappDefaultMessage,
        s.EmailFromName, s.EmailFromAddress, s.NotifyNewQuotation,
        s.SeoMetaTitle, s.SeoMetaDescription, s.SeoSocialImageUrl,
        s.LegalTermsUrl, s.LegalPrivacyUrl, s.LegalReturnsPolicy,
        s.CustomDomain, s.SslEnabled,
        s.AutoBackupEnabled, s.BackupFrequency, s.LastBackupDate,
        s.TwoFactorEnabled, s.SessionTimeoutMinutes
        );
}