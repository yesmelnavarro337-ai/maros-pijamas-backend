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
    private readonly ICacheService _cacheService;

    public PublicCategoriesController(ICategoryService categoryService, ICacheService cacheService)
    {
        _categoryService = categoryService;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _cacheService.GetOrCreateAsync(
            "public:categories",
            () => _categoryService.GetPublicAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(categories);
    }
}