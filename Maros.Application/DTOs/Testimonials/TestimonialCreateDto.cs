using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Testimonials;

public record TestimonialCreateDto(
    [Required, MaxLength(150)] string ClientName,
    [Range(1, 5)] int Rating,
    [Required, MaxLength(500)] string Quote
);