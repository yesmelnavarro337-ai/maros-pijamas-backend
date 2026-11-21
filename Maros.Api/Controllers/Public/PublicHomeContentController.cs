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
    private readonly ICacheService _cacheService;

    public PublicHomeContentController(IHomeSectionContentService service, ICacheService cacheService)
    {
        _service = service;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sections = await _cacheService.GetOrCreateAsync(
            "public:home-content",
            () => _service.GetPublicAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(sections);
    }
}
