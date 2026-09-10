using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly MarosDbContext _context;

    public BlogPostRepository(MarosDbContext context) => _context = context;

    public IQueryable<BlogPost> QueryAll() => _context.BlogPosts;

    public Task<BlogPost?> GetByIdAsync(Guid id) =>
        _context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
        _context.BlogPosts.AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId));

    public Task<List<BlogPost>> GetPublicAsync() =>
        _context.BlogPosts
            .Where(p => p.Status == BlogStatus.Publicado)
            .OrderByDescending(p => p.PublishDate)
            .ToListAsync();

    public async Task AddAsync(BlogPost post) =>
        await _context.BlogPosts.AddAsync(post);

    public void Remove(BlogPost post) =>
        _context.BlogPosts.Remove(post);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}