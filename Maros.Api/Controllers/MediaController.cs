using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireEditorOrAdmin")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Upload([FromForm] IFormFile? file, [FromForm] string? folder)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No se ha proporcionado ningún archivo válido." });

        try
        {
            var targetFolder = string.IsNullOrWhiteSpace(folder) ? "general" : folder;
            await using var stream = file.OpenReadStream();
            var result = await _mediaService.UploadAsync(stream, file.FileName, targetFolder);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al cargar la imagen: {ex.Message}" });
        }
    }
}