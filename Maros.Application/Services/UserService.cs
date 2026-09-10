using System.Security.Cryptography;
using Maros.Application.Common;
using Maros.Application.DTOs.Users;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(ToDto).ToList();
    }

    public async Task<UserResponseDto> InviteAsync(InviteUserRequestDto request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
            throw new AppException("Ya existe un usuario con ese correo electrónico.", 409);

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            throw new AppException("Rol inválido.", 400);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = role,
            Status = UserStatus.Pendiente,
        };

        // No existe todavía un flujo de invitación por correo (Fase futura de email).
        // Se genera una contraseña temporal aleatoria que el usuario no conoce;
        // el usuario queda en estado "Pendiente" hasta que se implemente
        // la aceptación de invitación real.
        var temporaryPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
        user.PasswordHash = _passwordHasher.Hash(temporaryPassword);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return ToDto(user);
    }

    public async Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new AppException("Usuario no encontrado.", 404);

        if (await _userRepository.EmailExistsAsync(request.Email, excludeUserId: id))
            throw new AppException("Ya existe otro usuario con ese correo electrónico.", 409);

        user.Name = request.Name;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task<UserResponseDto> UpdateRoleAsync(Guid id, UpdateUserRoleRequestDto request, Guid actingUserId)
    {
        if (id == actingUserId)
            throw new AppException("No puedes cambiar tu propio rol.", 403);

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new AppException("Usuario no encontrado.", 404);

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            throw new AppException("Rol inválido.", 400);

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task RemoveAsync(Guid id, Guid actingUserId)
    {
        if (id == actingUserId)
            throw new AppException("No puedes eliminarte a ti mismo del panel.", 403);

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new AppException("Usuario no encontrado.", 404);

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
    }

    private static UserResponseDto ToDto(User user) => new(
        user.Id,
        user.Name,
        user.Email,
        user.Role.ToString(),
        user.Status.ToString(),
        user.LastAccessAt
    );
}