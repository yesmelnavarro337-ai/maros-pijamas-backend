using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/page-headers")]
[AllowAnonymous]
public class PublicPageHeadersController : ControllerBase
{
    private readonly IPageHeaderService _pageHeaderService;
    private readonly ICacheService _cacheService;

    public PublicPageHeadersController(IPageHeaderService pageHeaderService, ICacheService cacheService)
    {
        _pageHeaderService = pageHeaderService;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var headers = await _cacheService.GetOrCreateAsync(
            "public:page-headers",
            () => _pageHeaderService.GetPublicAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(headers);
    }
}