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

    public PublicProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? categoryId, [FromQuery] Guid? collectionId, [FromQuery] string? search)
    {
        var products = await _productService.GetPublicListAsync(categoryId, collectionId, search);
        return Ok(products);
    }

    [HttpGet("featured-catalog")]
    public async Task<IActionResult> GetFeaturedCatalog()
    {
        var products = await _productService.GetFeaturedCatalogAsync();
        return Ok(products);
    }

    [HttpGet("customizable")]
    public async Task<IActionResult> GetCustomizable([FromQuery] int page = 1, [FromQuery] int pageSize = 6)
    {
        var result = await _productService.GetCustomizableAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var product = await _productService.GetPublicDetailBySlugAsync(slug);
        return Ok(product);
    }
}