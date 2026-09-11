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

    public PublicPageHeadersController(IPageHeaderService pageHeaderService)
    {
        _pageHeaderService = pageHeaderService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var headers = await _pageHeaderService.GetPublicAsync();
        return Ok(headers);
    }
}