using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/products")]
[AllowAnonymous]
public class PublicProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ICacheService _cacheService;

    public PublicProductsController(IProductService productService, ICacheService cacheService)
    {
        _productService = productService;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? collectionId,
        [FromQuery] string? search,
        [FromQuery] string? categories,
        [FromQuery] string? sizes,
        [FromQuery] string? colors)
    {
        var cacheKey = $"public:products:list:{categoryId}:{collectionId}:{search}:{categories}:{sizes}:{colors}";
        var products = await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _productService.GetPublicListAsync(categoryId, collectionId, search, categories, sizes, colors),
            TimeSpan.FromMinutes(3));
        return Ok(products);
    }

    [HttpGet("featured-catalog")]
    public async Task<IActionResult> GetFeaturedCatalog()
    {
        var products = await _cacheService.GetOrCreateAsync(
            "public:featured-catalog",
            () => _productService.GetFeaturedCatalogAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(products);
    }

    [HttpGet("customizable")]
    public async Task<IActionResult> GetCustomizable([FromQuery] int page = 1, [FromQuery] int pageSize = 6)
    {
        var cacheKey = $"public:products:customizable:{page}:{pageSize}";
        var result = await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _productService.GetCustomizableAsync(page, pageSize),
            TimeSpan.FromMinutes(5));
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var cacheKey = $"public:product:slug:{slug.ToLowerInvariant()}";
        var product = await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _productService.GetPublicDetailBySlugAsync(slug),
            TimeSpan.FromMinutes(5));
        return Ok(product);
    }
}