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
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string folder)
    {
        if (file.Length == 0)
            return BadRequest(new { message = "No se recibió ningún archivo." });

        await using var stream = file.OpenReadStream();
        var result = await _mediaService.UploadAsync(stream, file.FileName, folder);

        return Ok(result);
    }
}