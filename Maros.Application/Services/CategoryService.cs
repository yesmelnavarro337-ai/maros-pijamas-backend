using Maros.Application.Common;
using Maros.Application.DTOs.Categories;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        var results = await _categoryRepository.GetAllWithCountAsync();
        return results.Select(r => ToDto(r.Category, r.ProductsCount)).ToList();
    }

    public async Task<CategoryResponseDto> GetByIdAsync(Guid id)
    {
        var result = await _categoryRepository.GetByIdWithCountAsync(id)
            ?? throw new AppException("Categoría no encontrada.", 404);
        return ToDto(result.Category, result.ProductsCount);
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto request)
    {
        var slug = SlugGenerator.Generate(request.Name);

        if (await _categoryRepository.SlugExistsAsync(slug))
            throw new AppException("Ya existe una categoría con un nombre equivalente.", 409);

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            IsActive = request.IsActive,
        };
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return ToDto(category, 0);
    }

    public async Task<CategoryResponseDto> UpdateAsync(Guid id, CategoryUpdateDto request)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new AppException("Categoría no encontrada.", 404);

        // Si se proporcionó un slug manual, validarlo; de lo contrario regenerar desde el nombre
        var slug = !string.IsNullOrWhiteSpace(request.Slug)
            ? request.Slug.Trim().ToLowerInvariant()
            : SlugGenerator.Generate(request.Name);

        if (await _categoryRepository.SlugExistsAsync(slug, excludeId: id))
            throw new AppException("Ya existe otra categoría con un nombre equivalente.", 409);

        category.Name = request.Name;
        category.Slug = slug;
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.SaveChangesAsync();

        var count = await _categoryRepository.HasProductsAsync(id)
            ? (await _categoryRepository.GetByIdWithCountAsync(id))?.ProductsCount ?? 0
            : 0;

        return ToDto(category, count);
    }

    public async Task RemoveAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new AppException("Categoría no encontrada.", 404);

        if (await _categoryRepository.HasProductsAsync(id))
            throw new AppException("No se puede eliminar: hay productos asignados a esta categoría.", 409);

        _categoryRepository.Remove(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task<List<CategoryPublicDto>> GetPublicAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryPublicDto(c.Id, c.Name, c.Slug)).ToList();
    }

    private static CategoryResponseDto ToDto(Category c, int productsCount) => new(
        c.Id,
        c.Name,
        c.Slug,
        c.Description,
        c.ImageUrl,
        c.IsActive,
        productsCount,
        c.CreatedAt,
        c.UpdatedAt
    );
}
