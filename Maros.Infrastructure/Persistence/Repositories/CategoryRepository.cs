using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MarosDbContext _context;

    public CategoryRepository(MarosDbContext context) => _context = context;

    public Task<List<Category>> GetAllAsync() =>
        _context.Categories.OrderBy(c => c.Name).ToListAsync();

    public async Task<List<(Category Category, int ProductsCount)>> GetAllWithCountAsync()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                Category = c,
                ProductsCount = _context.Products.Count(p => p.CategoryId == c.Id && !p.IsDeleted)
            })
            .ToListAsync();

        return categories.Select(x => (x.Category, x.ProductsCount)).ToList();
    }

    public Task<Category?> GetByIdAsync(Guid id) =>
        _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<(Category Category, int ProductsCount)?> GetByIdWithCountAsync(Guid id)
    {
        var result = await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new
            {
                Category = c,
                ProductsCount = _context.Products.Count(p => p.CategoryId == c.Id && !p.IsDeleted)
            })
            .FirstOrDefaultAsync();

        if (result == null) return null;
        return (result.Category, result.ProductsCount);
    }

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
        _context.Categories.AnyAsync(c => c.Slug == slug && (excludeId == null || c.Id != excludeId));

    public Task<bool> HasProductsAsync(Guid categoryId) =>
        _context.Products.AnyAsync(p => p.CategoryId == categoryId && !p.IsDeleted);

    public Task ClearProductReferencesAsync(Guid categoryId) =>
        _context.Products
            .IgnoreQueryFilters()
            .Where(p => p.CategoryId == categoryId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.CategoryId, (Guid?)null));

    public async Task AddAsync(Category category) =>
        await _context.Categories.AddAsync(category);

    public void Remove(Category category) =>
        _context.Categories.Remove(category);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
