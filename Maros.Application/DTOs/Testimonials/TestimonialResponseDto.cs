namespace Maros.Application.DTOs.Testimonials;

public record TestimonialResponseDto(Guid Id, string ClientName, int Rating, string Quote, string Status);