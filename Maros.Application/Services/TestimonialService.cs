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

    public async Task<PagedResult<TestimonialResponseDto>> GetAllAsync(TestimonialQueryParams query)
    {
        TestimonialStatus? statusEnum = null;
        if (!string.IsNullOrWhiteSpace(query.Status) && !query.Status.Equals("todos", StringComparison.OrdinalIgnoreCase))
        {
            if (Enum.TryParse<TestimonialStatus>(query.Status, ignoreCase: true, out var parsed))
            {
                statusEnum = parsed;
            }
        }

        var page = query.PageNumber < 1 ? 1 : query.PageNumber;
        var size = query.PageSize < 1 ? 10 : query.PageSize;

        var (items, totalCount) = await _repository.GetFilteredAsync(query.Search, statusEnum, page, size);
        var dtos = items.Select(ToDto).ToList();

        return new PagedResult<TestimonialResponseDto>(dtos, page, size, totalCount);
    }

    public async Task<TestimonialResponseDto> CreateAsync(TestimonialCreateDto request)
    {
        var status = TestimonialStatus.Publicado;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<TestimonialStatus>(request.Status, ignoreCase: true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var testimonial = new Testimonial
        {
            ClientName = request.ClientName.Trim(),
            City = request.City?.Trim(),
            Rating = Math.Clamp(request.Rating, 1, 5),
            Quote = request.Quote.Trim(),
            AvatarUrl = request.AvatarUrl?.Trim(),
            Status = status,
            PublishDate = request.PublishDate ?? DateTime.UtcNow,
        };

        await _repository.AddAsync(testimonial);
        await _repository.SaveChangesAsync();

        return ToDto(testimonial);
    }

    public async Task<TestimonialResponseDto> UpdateAsync(Guid id, TestimonialUpdateDto request)
    {
        var testimonial = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Testimonio no encontrado.", 404);

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<TestimonialStatus>(request.Status, ignoreCase: true, out var parsedStatus))
        {
            testimonial.Status = parsedStatus;
        }

        testimonial.ClientName = request.ClientName.Trim();
        testimonial.City = request.City?.Trim();
        testimonial.Rating = Math.Clamp(request.Rating, 1, 5);
        testimonial.Quote = request.Quote.Trim();
        testimonial.AvatarUrl = request.AvatarUrl?.Trim();
        if (request.PublishDate.HasValue)
        {
            testimonial.PublishDate = request.PublishDate.Value;
        }
        testimonial.UpdatedAt = DateTime.UtcNow;

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
        var affected = await _repository.DeleteByIdAsync(id);
        if (affected == 0)
            throw new AppException("Testimonio no encontrado.", 404);
    }

    public async Task<List<TestimonialPublicDto>> GetPublicAsync()
    {
        var testimonials = await _repository.GetPublicAsync();
        return testimonials.Select(t => new TestimonialPublicDto(t.ClientName, t.Rating, t.Quote)).ToList();
    }

    private static TestimonialResponseDto ToDto(Testimonial t) =>
        new(t.Id, t.ClientName, t.City, t.Rating, t.Quote, t.AvatarUrl, t.Status.ToString(), t.PublishDate, t.CreatedAt);
}
