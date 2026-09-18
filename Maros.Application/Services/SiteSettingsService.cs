using System.Text.Json;
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
        var settings = await _repository.GetAsync();
        if (settings == null)
        {
            settings = new Domain.Entities.SiteSettings();
            await _repository.AddAsync(settings);
            await _repository.SaveChangesAsync();
        }
        return ToDto(settings);
    }

    public async Task<SiteSettingsResponseDto> UpdateAsync(SiteSettingsUpdateDto request)
    {
        var settings = await _repository.GetAsync();
        if (settings == null)
        {
            settings = new Domain.Entities.SiteSettings();
            await _repository.AddAsync(settings);
        }

        if (request.SiteName != null) settings.SiteName = request.SiteName;
        if (request.Description != null) settings.Description = request.Description;
        if (request.Currency != null) settings.Currency = request.Currency;
        if (request.Timezone != null) settings.Timezone = request.Timezone;
        if (request.Language != null) settings.Language = request.Language;
        if (request.DateFormat != null) settings.DateFormat = request.DateFormat;
        if (request.MaintenanceMode.HasValue) settings.MaintenanceMode = request.MaintenanceMode.Value;
        if (request.LogoUrl != null) settings.LogoUrl = request.LogoUrl;
        if (request.FaviconUrl != null) settings.FaviconUrl = request.FaviconUrl;

        if (request.HomeSections != null)
            settings.HomeSectionsJson = JsonSerializer.Serialize(request.HomeSections);

        if (request.Instagram != null) settings.Instagram = request.Instagram;
        if (request.Facebook != null) settings.Facebook = request.Facebook;
        if (request.TikTok != null) settings.TikTok = request.TikTok;
        if (request.Whatsapp != null) settings.Whatsapp = request.Whatsapp;
        if (request.Youtube != null) settings.Youtube = request.Youtube;
        if (request.Twitter != null) settings.Twitter = request.Twitter;

        if (request.WhatsappNumber != null) settings.WhatsappNumber = request.WhatsappNumber;
        if (request.WhatsappDefaultMessage != null) settings.WhatsappDefaultMessage = request.WhatsappDefaultMessage;
        if (request.WhatsappButtonImageUrl != null) settings.WhatsappButtonImageUrl = request.WhatsappButtonImageUrl;
        if (request.WhatsappPosition != null) settings.WhatsappPosition = request.WhatsappPosition;
        if (request.WhatsappButtonEnabled.HasValue) settings.WhatsappButtonEnabled = request.WhatsappButtonEnabled.Value;

        if (request.ContactPhone != null) settings.ContactPhone = request.ContactPhone;
        if (request.ContactEmail != null) settings.ContactEmail = request.ContactEmail;
        if (request.Address != null) settings.Address = request.Address;
        if (request.BusinessHours != null) settings.BusinessHours = request.BusinessHours;
        if (request.MapImageUrl != null) settings.MapImageUrl = request.MapImageUrl;
        if (request.ShowLocation.HasValue) settings.ShowLocation = request.ShowLocation.Value;

        if (request.EmailFromName != null) settings.EmailFromName = request.EmailFromName;
        if (request.EmailFromAddress != null) settings.EmailFromAddress = request.EmailFromAddress;
        if (request.DefaultSubject != null) settings.DefaultSubject = request.DefaultSubject;
        if (request.AutoReplyMessage != null) settings.AutoReplyMessage = request.AutoReplyMessage;
        if (request.NotifyNewQuotation.HasValue) settings.NotifyNewQuotation = request.NotifyNewQuotation.Value;

        if (request.SeoMetaTitle != null) settings.SeoMetaTitle = request.SeoMetaTitle;
        if (request.SeoMetaDescription != null) settings.SeoMetaDescription = request.SeoMetaDescription;
        if (request.Keywords != null) settings.Keywords = request.Keywords;
        if (request.CanonicalUrl != null) settings.CanonicalUrl = request.CanonicalUrl;
        if (request.RobotsTag != null) settings.RobotsTag = request.RobotsTag;
        if (request.SeoSocialImageUrl != null) settings.SeoSocialImageUrl = request.SeoSocialImageUrl;

        if (request.LegalPrivacyPolicy != null) settings.LegalPrivacyPolicy = request.LegalPrivacyPolicy;
        if (request.LegalTermsAndConditions != null) settings.LegalTermsAndConditions = request.LegalTermsAndConditions;
        if (request.LegalCookiesPolicy != null) settings.LegalCookiesPolicy = request.LegalCookiesPolicy;
        if (request.LegalTermsUrl != null) settings.LegalTermsUrl = request.LegalTermsUrl;
        if (request.LegalPrivacyUrl != null) settings.LegalPrivacyUrl = request.LegalPrivacyUrl;
        if (request.LegalReturnsPolicy != null) settings.LegalReturnsPolicy = request.LegalReturnsPolicy;

        if (request.CustomDomain != null) settings.CustomDomain = request.CustomDomain;
        if (request.WwwRedirect.HasValue) settings.WwwRedirect = request.WwwRedirect.Value;
        if (request.ServerIp != null) settings.ServerIp = request.ServerIp;
        if (request.SslEnabled.HasValue) settings.SslEnabled = request.SslEnabled.Value;

        if (request.AutoBackupEnabled.HasValue) settings.AutoBackupEnabled = request.AutoBackupEnabled.Value;
        if (request.BackupFrequency != null) settings.BackupFrequency = request.BackupFrequency;
        if (request.BackupTime != null) settings.BackupTime = request.BackupTime;
        if (request.BackupRetentionDays != null) settings.BackupRetentionDays = request.BackupRetentionDays;

        if (request.TwoFactorEnabled.HasValue) settings.TwoFactorEnabled = request.TwoFactorEnabled.Value;
        if (request.MaxLoginAttempts.HasValue) settings.MaxLoginAttempts = request.MaxLoginAttempts.Value;
        if (request.LockoutDurationMinutes.HasValue) settings.LockoutDurationMinutes = request.LockoutDurationMinutes.Value;
        if (request.SecurityNotificationsEnabled.HasValue) settings.SecurityNotificationsEnabled = request.SecurityNotificationsEnabled.Value;
        if (request.SessionTimeoutMinutes.HasValue) settings.SessionTimeoutMinutes = request.SessionTimeoutMinutes.Value;

        settings.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(settings);
    }

    public async Task<SiteSettingsPublicDto> GetPublicAsync()
    {
        var settings = await _repository.GetAsync() ?? new Domain.Entities.SiteSettings();

        return new SiteSettingsPublicDto(
            settings.SiteName,
            settings.Description,
            settings.MaintenanceMode,
            settings.LogoUrl,
            settings.Instagram,
            settings.Facebook,
            settings.TikTok,
            settings.WhatsappNumber,
            settings.EmailFromAddress,
            settings.Address,
            settings.BusinessHours,
            settings.SeoMetaTitle,
            settings.SeoMetaDescription,
            settings.SeoSocialImageUrl,
            settings.LegalTermsUrl,
            settings.LegalPrivacyUrl,
            settings.LegalReturnsPolicy
        );
    }

    private static List<HomeSectionDto>? DeserializeSections(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<HomeSectionDto>>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static SiteSettingsResponseDto ToDto(Domain.Entities.SiteSettings s) => new(
        s.SiteName, s.Description, s.Currency, s.Timezone, s.Language, s.DateFormat, s.MaintenanceMode, s.LogoUrl, s.FaviconUrl,
        DeserializeSections(s.HomeSectionsJson) ?? [],
        s.Instagram, s.Facebook, s.TikTok, s.Whatsapp, s.Youtube, s.Twitter,
        s.WhatsappNumber, s.WhatsappDefaultMessage, s.WhatsappButtonImageUrl, s.WhatsappPosition, s.WhatsappButtonEnabled,
        s.ContactPhone, s.ContactEmail, s.Address, s.BusinessHours, s.MapImageUrl, s.ShowLocation,
        s.EmailFromName, s.EmailFromAddress, s.DefaultSubject, s.AutoReplyMessage, s.NotifyNewQuotation,
        s.SeoMetaTitle, s.SeoMetaDescription, s.Keywords, s.CanonicalUrl, s.RobotsTag, s.SeoSocialImageUrl,
        s.LegalPrivacyPolicy, s.LegalTermsAndConditions, s.LegalCookiesPolicy, s.LegalTermsUrl, s.LegalPrivacyUrl, s.LegalReturnsPolicy,
        s.CustomDomain, s.WwwRedirect, s.ServerIp, s.SslEnabled,
        s.AutoBackupEnabled, s.BackupFrequency, s.BackupTime, s.BackupRetentionDays, s.LastBackupDate, s.LastBackupSize,
        s.TwoFactorEnabled, s.MaxLoginAttempts, s.LockoutDurationMinutes, s.SecurityNotificationsEnabled, s.SessionTimeoutMinutes
    );
}
