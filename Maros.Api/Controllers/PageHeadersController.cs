using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/admin/page-headers")]
[Authorize(Policy = "RequireAdministrador")]
public class PageHeadersController : ControllerBase
{
    private readonly IPageHeaderService _pageHeaderService;

    public PageHeadersController(IPageHeaderService pageHeaderService)
    {
        _pageHeaderService = pageHeaderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var headers = await _pageHeaderService.GetAllAsync();
        return Ok(headers);
    }

    [HttpPut("{pageKey}")]
    public async Task<IActionResult> Update([FromRoute] string pageKey, [FromBody] Maros.Application.DTOs.PageHeaders.PageHeaderUpdateDto request)
    {
        var updated = await _pageHeaderService.UpdateAsync(pageKey, request);
        return Ok(updated);
    }
}