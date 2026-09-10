using Maros.Application.DTOs.Settings;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdministrador")]
public class SettingsController : ControllerBase
{
    private readonly ISiteSettingsService _settingsService;

    public SettingsController(ISiteSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var settings = await _settingsService.GetAsync();
        return Ok(settings);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SiteSettingsUpdateDto request)
    {
        var updated = await _settingsService.UpdateAsync(request);
        return Ok(updated);
    }
}