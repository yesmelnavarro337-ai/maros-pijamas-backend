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
    private readonly ICacheService _cacheService;

    public SeasonController(ISeasonService seasonService, ICacheService cacheService)
    {
        _seasonService = seasonService;
        _cacheService = cacheService;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var season = await _cacheService.GetOrCreateAsync(
            "public:season:active",
            () => _seasonService.GetActivePublicAsync(),
            TimeSpan.FromMinutes(5));

        if (season is null)
            return NotFound(new { message = "No hay ninguna temporada activa actualmente." });

        return Ok(season);
    }
}