using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Repositories;

public class TestimonialRepository : ITestimonialRepository
{
    private readonly MarosDbContext _context;

    public TestimonialRepository(MarosDbContext context) => _context = context;

    public Task<List<Testimonial>> GetAllAsync() =>
        _context.Testimonials.OrderByDescending(t => t.PublishDate).ThenByDescending(t => t.CreatedAt).ToListAsync();

    public async Task<(List<Testimonial> Items, int TotalCount)> GetFilteredAsync(string? search, TestimonialStatus? status, int pageNumber, int pageSize)
    {
        var query = _context.Testimonials.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(t =>
                t.ClientName.ToLower().Contains(term) ||
                t.Quote.ToLower().Contains(term) ||
                (t.City != null && t.City.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.PublishDate)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task<Testimonial?> GetByIdAsync(Guid id) =>
        _context.Testimonials.FirstOrDefaultAsync(t => t.Id == id);

    public Task<List<Testimonial>> GetPublicAsync() =>
        _context.Testimonials
            .Where(t => t.Status == TestimonialStatus.Publicado)
            .OrderByDescending(t => t.PublishDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Testimonial testimonial) =>
        await _context.Testimonials.AddAsync(testimonial);

    public void Remove(Testimonial testimonial) =>
        _context.Testimonials.Remove(testimonial);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}