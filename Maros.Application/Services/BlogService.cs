using Maros.Application.Common;
using Maros.Application.DTOs.Blog;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Maros.Application.Services;

public class BlogService : IBlogService
{
    private readonly IBlogPostRepository _repository;
    private readonly IPaginationService _paginationService;
    private readonly ILogger<BlogService> _logger;

    public BlogService(
        IBlogPostRepository repository,
        IPaginationService paginationService,
        ILogger<BlogService> logger)
    {
        _repository = repository;
        _paginationService = paginationService;
        _logger = logger;
    }

    public async Task<PagedResult<BlogPostResponseDto>> GetAllAsync(BlogQueryParams query)
    {
        query ??= new BlogQueryParams();

        var posts = _repository.QueryAll();

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<BlogStatus>(query.Status, ignoreCase: true, out var status))
        {
            posts = posts.Where(p => p.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.Category) && query.Category != "todas")
        {
            posts = posts.Where(p => p.Category != null && p.Category == query.Category);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            posts = posts.Where(p =>
                (p.Title != null && p.Title.Contains(search)) ||
                (p.Content != null && p.Content.Contains(search)) ||
                (p.Category != null && p.Category.Contains(search))
            );
        }

        posts = query.SortDescending
            ? posts.OrderByDescending(p => p.PublishDate)
            : posts.OrderBy(p => p.PublishDate);

        var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        var paged = await _paginationService.PaginateAsync(posts, pageNumber, pageSize);
        var items = paged.Items != null
            ? paged.Items.Where(p => p != null).Select(ToDto).ToList()
            : new List<BlogPostResponseDto>();

        return new PagedResult<BlogPostResponseDto>(
            items,
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount);
    }

    public async Task<BlogPostResponseDto> GetByIdAsync(Guid id)
    {
        var post = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Artículo de blog no encontrado.", 404);
        return ToDto(post);
    }

    public async Task<BlogPostResponseDto> CreateAsync(BlogPostCreateDto request)
    {
        try
        {
            if (request == null)
                throw new AppException("Datos de entrada requeridos.", 400);

            var title = string.IsNullOrWhiteSpace(request.Title)
                ? "Nueva publicación"
                : request.Title.Trim();
            var category = string.IsNullOrWhiteSpace(request.Category)
                ? "General"
                : request.Category.Trim();
            var content = string.IsNullOrWhiteSpace(request.Content)
                ? "Próximamente..."
                : request.Content.Trim();
            var status = ParseStatus(request.Status);
            var slug = await GenerateUniqueSlugAsync(title);
            var coverImageUrl = NormalizeCloudinaryUrl(request.CoverImageUrl);

            var post = new BlogPost
            {
                Title = title,
                Slug = slug,
                Category = category,
                CoverImageUrl = coverImageUrl,
                Content = content,
                Status = status,
                PublishDate = request.PublishDate ?? DateTime.UtcNow,
            };

            await _repository.AddAsync(post);
            await _repository.SaveChangesAsync();

            return ToDto(post);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creando BlogPost. Message={Message}; Inner={Inner}",
                ex.Message,
                ex.InnerException?.Message);
            throw;
        }
    }

    public async Task<BlogPostResponseDto> UpdateAsync(Guid id, BlogPostUpdateDto request)
    {
        if (request == null)
            throw new AppException("Datos de entrada requeridos.", 400);

        var post = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Artículo no encontrado.", 404);

        var status = ParseStatus(request.Status);
        var slug = await GenerateUniqueSlugAsync(request.Title ?? post.Title, id);

        post.Title = request.Title ?? string.Empty;
        post.Slug = slug;
        post.Category = request.Category ?? string.Empty;
        post.CoverImageUrl = NormalizeCloudinaryUrl(request.CoverImageUrl);
        post.Content = request.Content ?? string.Empty;
        post.Status = status;
        post.PublishDate = request.PublishDate ?? post.PublishDate;
        post.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(post);
    }

    public async Task RemoveAsync(Guid id)
    {
        var post = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Artículo no encontrado.", 404);

        _repository.Remove(post);
        await _repository.SaveChangesAsync();
    }

    public async Task<List<BlogPostPublicDto>> GetPublicAsync()
    {
        var posts = await _repository.GetPublicAsync();
        return (posts ?? new List<BlogPost>())
            .Where(p => p != null)
            .Select(p => new BlogPostPublicDto(
                p.Title ?? string.Empty,
                p.Slug ?? string.Empty,
                p.Category ?? string.Empty,
                p.CoverImageUrl,
                p.Content ?? string.Empty,
                p.PublishDate
            )).ToList();
    }

    private static BlogStatus ParseStatus(string? input)
    {
        if (string.IsNullOrWhiteSpace(input) || !Enum.TryParse<BlogStatus>(input, ignoreCase: true, out var status))
            return BlogStatus.Borrador;
        return status;
    }

    private async Task<string> GenerateUniqueSlugAsync(string title, Guid? excludeId = null)
    {
        var slug = SlugGenerator.Generate(title);
        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = $"blog-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        if (await _repository.SlugExistsAsync(slug, excludeId))
        {
            slug = $"{slug}-{Guid.NewGuid().ToString()[..4]}";
        }

        return slug;
    }

    private static string? NormalizeCloudinaryUrl(string? coverImageUrl)
    {
        if (string.IsNullOrWhiteSpace(coverImageUrl))
            return null;

        var trimmed = coverImageUrl.Trim();
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
            throw new AppException("La imagen principal del blog debe ser una URL absoluta válida.", 400);

        if (!uri.Host.EndsWith("cloudinary.com", StringComparison.OrdinalIgnoreCase))
            throw new AppException("La imagen principal del blog debe subirse primero a Cloudinary.", 400);

        return trimmed;
    }

    private static BlogPostResponseDto ToDto(BlogPost p) => new(
        p.Id,
        p.Title ?? string.Empty,
        p.Slug ?? string.Empty,
        p.Category ?? string.Empty,
        p.CoverImageUrl,
        p.Content ?? string.Empty,
        p.Status.ToString(),
        p.PublishDate
    );
}
