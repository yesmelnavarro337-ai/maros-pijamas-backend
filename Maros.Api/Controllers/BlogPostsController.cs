using Maros.Application.Common;
using Maros.Application.DTOs.Blog;
using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogService _blogService;
    private readonly ILogger<BlogPostsController> _logger;

    public BlogPostsController(IBlogService blogService, ILogger<BlogPostsController> logger)
    {
        _blogService = blogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BlogQueryParams query)
    {
        try
        {
            var posts = await _blogService.GetAllAsync(query);
            return Ok(posts);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Excepción de aplicación al consultar BlogPosts: {Message}", ex.Message);
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error 500 no controlado en GET /api/BlogPosts. Message={Message}; Inner={Inner}",
                ex.Message,
                ex.InnerException?.Message);
            return StatusCode(500, new { message = "Error interno al procesar publicaciones del blog", error = ex.Message, innerError = ex.InnerException?.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var post = await _blogService.GetByIdAsync(id);
            return Ok(post);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Excepción de aplicación al consultar BlogPost {Id}: {Message}", id, ex.Message);
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error 500 no controlado en GET /api/BlogPosts/{Id}. Message={Message}; Inner={Inner}",
                id,
                ex.Message,
                ex.InnerException?.Message);
            return StatusCode(500, new { message = "Error interno al obtener publicación del blog", error = ex.Message, innerError = ex.InnerException?.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Create([FromBody] BlogPostCreateDto request)
    {
        try
        {
            var created = await _blogService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Excepción de aplicación al crear BlogPost: {Message}", ex.Message);
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error 500 no controlado en POST /api/BlogPosts. Message={Message}; Inner={Inner}",
                ex.Message,
                ex.InnerException?.Message);
            return StatusCode(500, new { message = "Error interno al crear publicación del blog", error = ex.Message, innerError = ex.InnerException?.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] BlogPostUpdateDto request)
    {
        try
        {
            var updated = await _blogService.UpdateAsync(id, request);
            return Ok(updated);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Excepción de aplicación al actualizar BlogPost {Id}: {Message}", id, ex.Message);
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error 500 no controlado en PUT /api/BlogPosts/{Id}. Message={Message}; Inner={Inner}",
                id,
                ex.Message,
                ex.InnerException?.Message);
            return StatusCode(500, new { message = "Error interno al actualizar publicación del blog", error = ex.Message, innerError = ex.InnerException?.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireEditorOrAdmin")]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            await _blogService.RemoveAsync(id);
            return NoContent();
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Excepción de aplicación al eliminar BlogPost {Id}: {Message}", id, ex.Message);
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error 500 no controlado en DELETE /api/BlogPosts/{Id}. Message={Message}; Inner={Inner}",
                id,
                ex.Message,
                ex.InnerException?.Message);
            return StatusCode(500, new { message = "Error interno al eliminar publicación del blog", error = ex.Message, innerError = ex.InnerException?.Message });
        }
    }
}
