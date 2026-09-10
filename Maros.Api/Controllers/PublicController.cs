using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public/[controller]")]
[AllowAnonymous]
public class CollectionController : ControllerBase
{
    private readonly ICollectionService _collectionService;

    public CollectionController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var collection = await _collectionService.GetActivePublicAsync();
        return Ok(collection);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var collections = await _collectionService.GetAllPublicAsync();
        return Ok(collections);
    }
}