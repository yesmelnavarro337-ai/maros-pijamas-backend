using Maros.Application.Common;
using Maros.Application.DTOs.Blog;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class BlogService : IBlogService
{
    private readonly IBlogPostRepository _repository;
    private readonly IPaginationService _paginationService;

    public BlogService(IBlogPostRepository repository, IPaginationService paginationService)
    {
        _repository = repository;
        _paginationService = paginationService;
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
        if (request == null)
            throw new AppException("Datos de entrada requeridos.", 400);

        var status = ParseStatus(request.Status);
        var slug = SlugGenerator.Generate(request.Title ?? string.Empty);

        if (await _repository.SlugExistsAsync(slug))
        {
            slug = $"{slug}-{Guid.NewGuid().ToString()[..4]}";
        }

        var post = new BlogPost
        {
            Title = request.Title ?? string.Empty,
            Slug = slug,
            Category = request.Category ?? string.Empty,
            CoverImageUrl = request.CoverImageUrl,
            Content = request.Content ?? string.Empty,
            Status = status,
            PublishDate = request.PublishDate,
        };

        await _repository.AddAsync(post);
        await _repository.SaveChangesAsync();

        return ToDto(post);
    }

    public async Task<BlogPostResponseDto> UpdateAsync(Guid id, BlogPostUpdateDto request)
    {
        if (request == null)
            throw new AppException("Datos de entrada requeridos.", 400);

        var post = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Artículo no encontrado.", 404);

        var status = ParseStatus(request.Status);
        var slug = SlugGenerator.Generate(request.Title ?? string.Empty);

        if (await _repository.SlugExistsAsync(slug, excludeId: id))
        {
            slug = $"{slug}-{Guid.NewGuid().ToString()[..4]}";
        }

        post.Title = request.Title ?? string.Empty;
        post.Slug = slug;
        post.Category = request.Category ?? string.Empty;
        post.CoverImageUrl = request.CoverImageUrl;
        post.Content = request.Content ?? string.Empty;
        post.Status = status;
        post.PublishDate = request.PublishDate;
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