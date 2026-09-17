namespace Maros.Application.DTOs.Testimonials;

public record TestimonialUpdateDto(
    string ClientName,
    string? City,
    int Rating,
    string Quote,
    string? AvatarUrl,
    string Status,
    DateTime? PublishDate
);