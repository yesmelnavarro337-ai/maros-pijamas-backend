using Maros.Application.Interfaces;
using Maros.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maros.Application.DTOs.Gallery;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GalleryQueryParams query)
    {
        var images = await _galleryService.GetAllAsync(query);
        return Ok(images);
    }

    [HttpPost]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    [RequestSizeLimit(10_000_000)] // 10 MB máximo por imagen
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string category, [FromForm] string caption)
    {
        if (file.Length == 0)
            return BadRequest(new { message = "No se recibió ningún archivo." });

        if (!Enum.TryParse<GalleryCategory>(category, ignoreCase: true, out var parsedCategory))
            return BadRequest(new { message = "Categoría inválida." });

        await using var stream = file.OpenReadStream();
        var uploaded = await _galleryService.UploadAsync(stream, file.FileName, parsedCategory, caption);

        return CreatedAtAction(nameof(GetAll), uploaded);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _galleryService.RemoveAsync(id);
        return NoContent();
    }
}