using Maros.Application.Interfaces;
using Maros.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/gallery")]
[AllowAnonymous]
public class PublicGalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;

    public PublicGalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGallery([FromQuery] string? category)
    {
        GalleryCategory? parsed = null;
        if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<GalleryCategory>(category, ignoreCase: true, out var value))
            parsed = value;

        var images = await _galleryService.GetPublicAsync(parsed);
        return Ok(images);
    }
}