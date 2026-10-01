using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers.Public;

[ApiController]
[Route("api/public/home-content")]
[AllowAnonymous]
public class PublicHomeContentController : ControllerBase
{
    private readonly IHomeSectionContentService _service;

    public PublicHomeContentController(IHomeSectionContentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sections = await _service.GetPublicAsync();
        return Ok(sections);
    }
}
