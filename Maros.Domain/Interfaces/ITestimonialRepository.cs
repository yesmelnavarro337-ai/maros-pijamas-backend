using Maros.Domain.Entities;
using Maros.Domain.Enums;

namespace Maros.Domain.Interfaces;

public interface ITestimonialRepository
{
    Task<List<Testimonial>> GetAllAsync();
    Task<(List<Testimonial> Items, int TotalCount)> GetFilteredAsync(string? search, TestimonialStatus? status, int pageNumber, int pageSize);
    Task<Testimonial?> GetByIdAsync(Guid id);
    Task<List<Testimonial>> GetPublicAsync();
    Task AddAsync(Testimonial testimonial);
    void Remove(Testimonial testimonial);
    Task SaveChangesAsync();
}