using Maros.Application.DTOs.Collections;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollectionsController : ControllerBase
{
    private readonly ICollectionService _collectionService;

    public CollectionsController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var collections = await _collectionService.GetAllAsync();
        return Ok(collections);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var collection = await _collectionService.GetByIdAsync(id);
        return Ok(collection);
    }

    [HttpPost]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CollectionCreateDto request)
    {
        var created = await _collectionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CollectionUpdateDto request)
    {
        var updated = await _collectionService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/set-default")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> SetDefault(Guid id)
    {
        var updated = await _collectionService.SetDefaultAsync(id);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _collectionService.RemoveAsync(id);
        return Ok(new { success = true, message = "Colección eliminada correctamente." });
    }
}