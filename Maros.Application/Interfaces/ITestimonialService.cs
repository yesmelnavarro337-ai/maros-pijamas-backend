using Maros.Application.DTOs.Testimonials;

namespace Maros.Application.Interfaces;

public interface ITestimonialService
{
    Task<List<TestimonialResponseDto>> GetAllAsync();
    Task<TestimonialResponseDto> CreateAsync(TestimonialCreateDto request);
    Task<TestimonialResponseDto> UpdateStatusAsync(Guid id, TestimonialUpdateDto request);
    Task RemoveAsync(Guid id);
    Task<List<TestimonialPublicDto>> GetPublicAsync();
}