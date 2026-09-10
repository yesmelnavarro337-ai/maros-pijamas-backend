using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/categories")]
[AllowAnonymous]
public class PublicCategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public PublicCategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetPublicAsync();
        return Ok(categories);
    }
}