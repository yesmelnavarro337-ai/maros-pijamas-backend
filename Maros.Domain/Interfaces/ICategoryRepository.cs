using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<bool> HasProductsAsync(Guid categoryId);
    Task AddAsync(Category category);
    void Remove(Category category);
    Task SaveChangesAsync();
}