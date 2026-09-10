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
        _context.Testimonials.OrderByDescending(t => t.CreatedAt).ToListAsync();

    public Task<Testimonial?> GetByIdAsync(Guid id) =>
        _context.Testimonials.FirstOrDefaultAsync(t => t.Id == id);

    public Task<List<Testimonial>> GetPublicAsync() =>
        _context.Testimonials
            .Where(t => t.Status == TestimonialStatus.Publicado)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Testimonial testimonial) =>
        await _context.Testimonials.AddAsync(testimonial);

    public void Remove(Testimonial testimonial) =>
        _context.Testimonials.Remove(testimonial);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}