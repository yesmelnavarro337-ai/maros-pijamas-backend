using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedInitialAdminAsync(MarosDbContext context, IConfiguration configuration)
    {
        var email = configuration["SeedAdmin:Email"] ?? "yesmelnavarro337@gmail.com";
        var password = configuration["SeedAdmin:Password"] ?? "Yesmel3110.";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var hasher = new PasswordHasher<User>();

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (existingUser is not null)
        {
            // Resetear contraseña y asegurar rol Administrador
            existingUser.PasswordHash = hasher.HashPassword(existingUser, password);
            existingUser.Role = UserRole.Administrador;
            existingUser.Status = UserStatus.Activo;
            Console.WriteLine($"[SeedData] Contraseña del usuario administrador '{email}' restablecida exitosamente.");
        }
        else
        {
            // Crear usuario administrador si no existe
            var admin = new User
            {
                Name = "Yesmel Navarro",
                Email = email,
                Role = UserRole.Administrador,
                Status = UserStatus.Activo,
            };
            admin.PasswordHash = hasher.HashPassword(admin, password);
            context.Users.Add(admin);
            Console.WriteLine($"[SeedData] Usuario administrador '{email}' creado exitosamente.");
        }

        await context.SaveChangesAsync();
    }
}