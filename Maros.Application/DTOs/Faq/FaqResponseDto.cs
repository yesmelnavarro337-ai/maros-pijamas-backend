namespace Maros.Application.DTOs.Faq;

public record FaqResponseDto(Guid Id, string Question, string Answer, string Category, int Order, string Status);