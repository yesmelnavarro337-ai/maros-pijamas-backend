using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    /// <summary>Health check ligero para keep-alive / monitoreo (sin acceso a BD).</summary>
    [HttpGet]
    [HttpHead]
    public IActionResult Get()
    {
        return Ok(new { status = "ok" });
    }
}
