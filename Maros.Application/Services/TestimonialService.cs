using Maros.Application.Common;
using Maros.Application.DTOs.Testimonials;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class TestimonialService : ITestimonialService
{
    private readonly ITestimonialRepository _repository;

    public TestimonialService(ITestimonialRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TestimonialResponseDto>> GetAllAsync()
    {
        var testimonials = await _repository.GetAllAsync();
        return testimonials.Select(ToDto).ToList();
    }

    public async Task<TestimonialResponseDto> CreateAsync(TestimonialCreateDto request)
    {
        var testimonial = new Testimonial
        {
            ClientName = request.ClientName,
            Rating = request.Rating,
            Quote = request.Quote,
            Status = TestimonialStatus.Publicado,
        };

        await _repository.AddAsync(testimonial);
        await _repository.SaveChangesAsync();

        return ToDto(testimonial);
    }

    public async Task<TestimonialResponseDto> UpdateStatusAsync(Guid id, TestimonialUpdateDto request)
    {
        var testimonial = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Testimonio no encontrado.", 404);

        if (!Enum.TryParse<TestimonialStatus>(request.Status, ignoreCase: true, out var status))
            throw new AppException("Estado de testimonio inválido.", 400);

        testimonial.Status = status;
        testimonial.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(testimonial);
    }

    public async Task RemoveAsync(Guid id)
    {
        var testimonial = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Testimonio no encontrado.", 404);

        _repository.Remove(testimonial);
        await _repository.SaveChangesAsync();
    }

    public async Task<List<TestimonialPublicDto>> GetPublicAsync()
    {
        var testimonials = await _repository.GetPublicAsync();
        return testimonials.Select(t => new TestimonialPublicDto(t.ClientName, t.Rating, t.Quote)).ToList();
    }

    private static TestimonialResponseDto ToDto(Testimonial t) =>
        new(t.Id, t.ClientName, t.Rating, t.Quote, t.Status.ToString());
}