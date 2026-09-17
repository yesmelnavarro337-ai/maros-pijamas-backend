using Maros.Application.Common;
using Maros.Application.DTOs.Testimonials;

namespace Maros.Application.Interfaces;

public interface ITestimonialService
{
    Task<PagedResult<TestimonialResponseDto>> GetAllAsync(TestimonialQueryParams query);
    Task<TestimonialResponseDto> CreateAsync(TestimonialCreateDto request);
    Task<TestimonialResponseDto> UpdateAsync(Guid id, TestimonialUpdateDto request);
    Task<TestimonialResponseDto> UpdateStatusAsync(Guid id, TestimonialUpdateDto request);
    Task RemoveAsync(Guid id);
    Task<List<TestimonialPublicDto>> GetPublicAsync();
}