using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Faq;

public record FaqCreateDto(
    [Required, MaxLength(300)] string Question,
    [Required, MaxLength(1000)] string Answer,
    [Required, MaxLength(60)] string Category,
    [Required] string Status
);