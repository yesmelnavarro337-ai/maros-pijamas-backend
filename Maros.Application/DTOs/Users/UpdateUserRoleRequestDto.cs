using System.ComponentModel.DataAnnotations;

namespace Maros.Application.DTOs.Users;

public record UpdateUserRoleRequestDto([Required] string Role);