using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<List<(Category Category, int ProductsCount)>> GetAllWithCountAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<(Category Category, int ProductsCount)?> GetByIdWithCountAsync(Guid id);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<bool> HasProductsAsync(Guid categoryId);
    Task ClearProductReferencesAsync(Guid categoryId);
    Task AddAsync(Category category);
    void Remove(Category category);
    Task SaveChangesAsync();
}
