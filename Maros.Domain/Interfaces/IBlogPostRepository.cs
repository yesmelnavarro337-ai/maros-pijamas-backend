using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface IBlogPostRepository
{
    IQueryable<BlogPost> QueryAll();
    Task<BlogPost?> GetByIdAsync(Guid id);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<List<BlogPost>> GetPublicAsync();
    Task AddAsync(BlogPost post);
    void Remove(BlogPost post);
    Task SaveChangesAsync();
}