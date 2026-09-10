using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.ContactMessages;

public record ContactMessageCreateDto(
    [Required, MaxLength(150)] string FullName,
    [Required, MaxLength(30)] string Phone,
    [MaxLength(256)] string? Email,
    [Required, MinLength(10), MaxLength(1000)] string Message
);