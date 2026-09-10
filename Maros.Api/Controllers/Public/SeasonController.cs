using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/[controller]")]
[AllowAnonymous]
public class SeasonController : ControllerBase
{
    private readonly ISeasonService _seasonService;

    public SeasonController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var season = await _seasonService.GetActivePublicAsync();

        if (season is null)
            return NotFound(new { message = "No hay ninguna temporada activa actualmente." });

        return Ok(season);
    }
}