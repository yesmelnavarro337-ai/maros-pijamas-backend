namespace Maros.Application.DTOs.Testimonials;

public record TestimonialResponseDto(
    Guid Id,
    string ClientName,
    string? City,
    int Rating,
    string Quote,
    string? AvatarUrl,
    string Status,
    DateTime PublishDate,
    DateTime CreatedAt
);