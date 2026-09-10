using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class Testimonial : BaseEntity
{
    public string ClientName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Quote { get; set; } = string.Empty;
    public TestimonialStatus Status { get; set; } = TestimonialStatus.Publicado;
}