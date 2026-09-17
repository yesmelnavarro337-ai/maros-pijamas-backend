using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class Testimonial : BaseEntity
{
    public string ClientName { get; set; } = string.Empty;
    public string? City { get; set; }
    public int Rating { get; set; } = 5;
    public string Quote { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public TestimonialStatus Status { get; set; } = TestimonialStatus.Publicado;
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
}