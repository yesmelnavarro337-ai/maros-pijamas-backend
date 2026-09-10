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

    public Task<Category?> GetByIdAsync(Guid id) =>
        _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
        _context.Categories.AnyAsync(c => c.Slug == slug && (excludeId == null || c.Id != excludeId));

    public Task<bool> HasProductsAsync(Guid categoryId) =>
        _context.Products.AnyAsync(p => p.CategoryId == categoryId);

    public async Task AddAsync(Category category) =>
        await _context.Categories.AddAsync(category);

    public void Remove(Category category) =>
        _context.Categories.Remove(category);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}