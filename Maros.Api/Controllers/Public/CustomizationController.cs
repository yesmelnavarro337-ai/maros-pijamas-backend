using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/[controller]")]
[AllowAnonymous]
public class CustomizationController : ControllerBase
{
    private readonly ICustomizationOptionService _service;

    public CustomizationController(ICustomizationOptionService service)
    {
        _service = service;
    }

    [HttpGet("options")]
    public async Task<IActionResult> GetOptions()
    {
        var catalog = await _service.GetPublicCatalogAsync();
        return Ok(catalog);
    }
}