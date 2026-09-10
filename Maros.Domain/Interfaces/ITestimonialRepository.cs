using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ITestimonialRepository
{
    Task<List<Testimonial>> GetAllAsync();
    Task<Testimonial?> GetByIdAsync(Guid id);
    Task<List<Testimonial>> GetPublicAsync();
    Task AddAsync(Testimonial testimonial);
    void Remove(Testimonial testimonial);
    Task SaveChangesAsync();
}