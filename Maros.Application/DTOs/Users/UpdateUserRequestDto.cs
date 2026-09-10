using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Users;

public record UpdateUserRequestDto(
    [Required, MaxLength(200)] string Name,
    [Required, EmailAddress, MaxLength(256)] string Email
);