using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireEditorOrAdmin")]
public class MediaController : ControllerBase
{
    // Extensiones aceptadas por el upload unificado (imágenes + videos).
    // Incluye los formatos Apple de iPhone: .mov y .quicktime (video/quicktime).
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Imágenes
        ".jpg", ".jpeg", ".png", ".webp", ".gif", ".heic", ".heif",
        ".bmp", ".avif", ".tiff", ".svg",
        // Videos (incluye MOV/QuickTime de iPhone)
        ".mp4", ".webm", ".mov", ".m4v", ".avi", ".mkv", ".quicktime"
    };

    private readonly IMediaService _mediaService;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IMediaService mediaService, ILogger<MediaController> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(100_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
    public async Task<IActionResult> Upload([FromForm] IFormFile? file, [FromForm] string? folder)
    {
        if (file is null || file.Length == 0)
        {
            _logger.LogWarning("Petición de carga recibida sin ningún archivo adjunto.");
            return BadRequest(new { status = 400, message = "No se ha proporcionado ningún archivo válido." });
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Extensión no permitida en carga de archivo '{FileName}'.", file.FileName);
            return BadRequest(new
            {
                status = 400,
                message = $"Extensión '{extension}' no permitida. Formatos aceptados: {string.Join(", ", AllowedExtensions.Order())}."
            });
        }

        try
        {
            var targetFolder = string.IsNullOrWhiteSpace(folder) ? "general" : folder;
            _logger.LogInformation("Iniciando procesamiento de archivo '{FileName}' ({Length} bytes) hacia carpeta '{Folder}'",
                file.FileName, file.Length, targetFolder);

            await using var stream = file.OpenReadStream();
            var result = await _mediaService.UploadAsync(stream, file.FileName, targetFolder);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando carga de archivo '{FileName}' en MediaController.Upload", file.FileName);
            return StatusCode(500, new { status = 500, message = ex.Message });
        }
    }
}