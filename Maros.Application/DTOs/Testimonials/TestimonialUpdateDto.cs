using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Testimonials;

public record TestimonialUpdateDto([Required] string Status);