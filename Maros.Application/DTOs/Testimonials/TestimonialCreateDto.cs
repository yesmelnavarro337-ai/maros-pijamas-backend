namespace Maros.Application.DTOs.Testimonials;

public record TestimonialCreateDto(
    string ClientName,
    string? City,
    int Rating,
    string Quote,
    string? AvatarUrl,
    string Status,
    DateTime? PublishDate
);