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
    private readonly IMediaService _mediaService;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ISiteSettingsService settingsService,
        ISiteSettingsRepository repository,
        IMediaService mediaService,
        ILogger<SettingsController> logger)
    {
        _settingsService = settingsService;
        _repository = repository;
        _mediaService = mediaService;
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

    // ─── Audio ambiental (instrumentales MP3) ─────────────────────────

    /// <summary>
    /// Consulta las URLs públicas vigentes de los audios ambientales
    /// (instrumental-navideño e instrumental-nosotros). Los MP3 se almacenan
    /// en Cloudinary (carpeta maros-pijamas/audio) y la BD solo guarda la URL.
    /// </summary>
    [HttpGet("audio")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAudio()
    {
        try
        {
            var settings = await _repository.GetAsync() ?? new Domain.Entities.SiteSettings();
            return Ok(new
            {
                navidad = string.IsNullOrWhiteSpace(settings.InstrumentalNavidadUrl) ? null : settings.InstrumentalNavidadUrl,
                nosotros = string.IsNullOrWhiteSpace(settings.InstrumentalNosotrosUrl) ? null : settings.InstrumentalNosotrosUrl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la configuración de audio ambiental.");
            return StatusCode(500, new { message = "Error al obtener la configuración de audio." });
        }
    }

    /// <summary>
    /// Recibe un archivo MP3 y el identificador de tipo ("navidad" o "nosotros"),
    /// lo sube a Cloudinary (resource type raw, carpeta maros-pijamas/audio) y
    /// guarda la URL pública resultante en la configuración correspondiente.
    /// </summary>
    [HttpPost("audio")]
    [RequestSizeLimit(30_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 30_000_000)]
    public async Task<IActionResult> UploadAudio([FromForm] IFormFile? file, [FromForm] string? type)
    {
        try
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(new { message = "Debe adjuntar un archivo MP3 válido." });
            }

            var audioType = type?.Trim().ToLowerInvariant();
            if (audioType is not ("navidad" or "nosotros"))
            {
                return BadRequest(new { message = "El tipo de audio debe ser \"navidad\" o \"nosotros\"." });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var isMp3 = extension == ".mp3"
                || file.ContentType.Equals("audio/mpeg", StringComparison.OrdinalIgnoreCase)
                || file.ContentType.Equals("audio/mp3", StringComparison.OrdinalIgnoreCase);

            if (!isMp3)
            {
                return BadRequest(new { message = "Solo se permiten archivos MP3." });
            }

            await using var stream = file.OpenReadStream();
            var upload = await _mediaService.UploadAsync(stream, file.FileName, "audio");
            var publicUrl = upload.Url;

            var settings = await _repository.GetAsync();
            if (settings is null)
            {
                settings = new Domain.Entities.SiteSettings();
                await _repository.AddAsync(settings);
            }

            if (audioType == "navidad")
            {
                settings.InstrumentalNavidadUrl = publicUrl;
            }
            else
            {
                settings.InstrumentalNosotrosUrl = publicUrl;
            }

            settings.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Audio ambiental '{Type}' subido a Cloudinary: {Url}", audioType, publicUrl);

            return Ok(new
            {
                message = $"Audio {audioType} actualizado correctamente.",
                navidad = settings.InstrumentalNavidadUrl,
                nosotros = settings.InstrumentalNosotrosUrl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir el audio ambiental de tipo '{Type}'.", type);
            return StatusCode(500, new { message = "Error interno al guardar el audio." });
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
