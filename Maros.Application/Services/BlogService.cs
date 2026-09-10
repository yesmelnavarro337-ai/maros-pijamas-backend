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
        var posts = _repository.QueryAll();

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<BlogStatus>(query.Status, ignoreCase: true, out var status))
            posts = posts.Where(p => p.Status == status);

        if (!string.IsNullOrWhiteSpace(query.Category))
            posts = posts.Where(p => p.Category == query.Category);

        if (!string.IsNullOrWhiteSpace(query.Search))
            posts = posts.Where(p => p.Title.Contains(query.Search));

        posts = query.SortDescending
            ? posts.OrderByDescending(p => p.PublishDate)
            : posts.OrderBy(p => p.PublishDate);

        var paged = await _paginationService.PaginateAsync(posts, query.PageNumber, query.PageSize);
        return new PagedResult<BlogPostResponseDto>(
    paged.Items.Select(ToDto).ToList(),
    paged.PageNumber,
    paged.PageSize,
    paged.TotalCount);  


    }

    public async Task<BlogPostResponseDto> CreateAsync(BlogPostCreateDto request)
    {
        var status = ParseStatus(request.Status);
        var slug = SlugGenerator.Generate(request.Title);

        if (await _repository.SlugExistsAsync(slug))
            throw new AppException("Ya existe un artículo con un título equivalente.", 409);

        var post = new BlogPost
        {
            Title = request.Title,
            Slug = slug,
            Category = request.Category,
            CoverImageUrl = request.CoverImageUrl,
            Content = request.Content,
            Status = status,
            PublishDate = request.PublishDate,
        };

        await _repository.AddAsync(post);
        await _repository.SaveChangesAsync();

        return ToDto(post);
    }

    public async Task<BlogPostResponseDto> UpdateAsync(Guid id, BlogPostUpdateDto request)
    {
        var post = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Artículo no encontrado.", 404);

        var status = ParseStatus(request.Status);
        var slug = SlugGenerator.Generate(request.Title);

        if (await _repository.SlugExistsAsync(slug, excludeId: id))
            throw new AppException("Ya existe otro artículo con un título equivalente.", 409);

        post.Title = request.Title;
        post.Slug = slug;
        post.Category = request.Category;
        post.CoverImageUrl = request.CoverImageUrl;
        post.Content = request.Content;
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
        return posts.Select(p => new BlogPostPublicDto(
            p.Title, p.Slug, p.Category, p.CoverImageUrl, p.Content, p.PublishDate
        )).ToList();
    }

    private static BlogStatus ParseStatus(string input)
    {
        if (!Enum.TryParse<BlogStatus>(input, ignoreCase: true, out var status))
            throw new AppException("Estado de artículo inválido.", 400);
        return status;
    }

    private static BlogPostResponseDto ToDto(BlogPost p) => new(
        p.Id, p.Title, p.Slug, p.Category, p.CoverImageUrl, p.Content, p.Status.ToString(), p.PublishDate
    );
}