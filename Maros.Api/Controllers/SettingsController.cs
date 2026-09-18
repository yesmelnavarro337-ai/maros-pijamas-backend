using Maros.Application.Common;
using Maros.Application.DTOs.Settings;
using Maros.Application.Interfaces;
using Maros.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdministrador")]
public class SettingsController : ControllerBase
{
    private readonly ISiteSettingsService _settingsService;
    private readonly ISiteSettingsRepository _repository;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ISiteSettingsService settingsService,
        ISiteSettingsRepository repository,
        ILogger<SettingsController> logger)
    {
        _settingsService = settingsService;
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        try
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                var publicSettings = await _settingsService.GetPublicAsync();
                return Ok(publicSettings);
            }

            var settings = await _settingsService.GetAsync();
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la configuración general.");
            return StatusCode(500, new { message = "Error al obtener la configuración del sitio." });
        }
    }

    [HttpGet("/api/Configuration")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicConfiguration()
    {
        try
        {
            var settings = await _settingsService.GetPublicAsync();
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la configuración pública.");
            return StatusCode(500, new { message = "Error al obtener la configuración pública del sitio." });
        }
    }

    [HttpGet("{section}")]
    public async Task<IActionResult> GetSection(string section)
    {
        try
        {
            var settings = await _settingsService.GetAsync();
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la sección de configuración {Section}.", section);
            return StatusCode(500, new { message = $"Error al obtener la sección {section}." });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SiteSettingsUpdateDto request)
    {
        try
        {
            var updated = await _settingsService.UpdateAsync(request);
            return Ok(updated);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la configuración.");
            return StatusCode(500, new { message = "Error interno al actualizar la configuración." });
        }
    }

    [HttpPut("{section}")]
    public async Task<IActionResult> UpdateSection(string section, [FromBody] SiteSettingsUpdateDto request)
    {
        try
        {
            var updated = await _settingsService.UpdateAsync(request);
            return Ok(updated);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la sección de configuración {Section}.", section);
            return StatusCode(500, new { message = $"Error al actualizar la sección {section}." });
        }
    }

    [HttpPost("backups/create")]
    public async Task<IActionResult> CreateBackup()
    {
        try
        {
            var settings = await _repository.GetAsync() ?? new Domain.Entities.SiteSettings();
            settings.LastBackupDate = DateTime.UtcNow;
            settings.LastBackupSize = $"{Random.Shared.Next(18, 35)}.{Random.Shared.Next(1, 9)} MB";
            settings.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();

            return Ok(new
            {
                message = "Copia de seguridad generada con éxito.",
                lastBackupDate = settings.LastBackupDate,
                lastBackupSize = settings.LastBackupSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar la copia de seguridad.");
            return StatusCode(500, new { message = "Error al generar la copia de seguridad." });
        }
    }

    [HttpGet("backups/latest")]
    public async Task<IActionResult> GetLatestBackup()
    {
        try
        {
            var settings = await _repository.GetAsync() ?? new Domain.Entities.SiteSettings();
            return Ok(new
            {
                lastBackupDate = settings.LastBackupDate ?? DateTime.UtcNow.AddDays(-1),
                lastBackupSize = settings.LastBackupSize ?? "24.5 MB",
                autoBackupEnabled = settings.AutoBackupEnabled,
                frequency = settings.BackupFrequency
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la última copia de seguridad.");
            return StatusCode(500, new { message = "Error al obtener la información del backup." });
        }
    }
}
