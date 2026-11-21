using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/home")]
[AllowAnonymous]
public class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;
    private readonly ICacheService _cacheService;

    public HomeController(IHomeService homeService, ICacheService cacheService)
    {
        _homeService = homeService;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var home = await _cacheService.GetOrCreateAsync(
            "public:home",
            () => _homeService.GetHomeAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(home);
    }
}