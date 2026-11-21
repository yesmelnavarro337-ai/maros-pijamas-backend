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
            // Solo actualizar si no tiene el rol de Administrador o no está Activo, evitando escrituras innecesarias en cada inicio
            if (existingUser.Role != UserRole.Administrador || existingUser.Status != UserStatus.Activo)
            {
                existingUser.Role = UserRole.Administrador;
                existingUser.Status = UserStatus.Activo;
                await context.SaveChangesAsync();
                Console.WriteLine($"[SeedData] Rol y estado del usuario administrador '{email}' asegurados.");
            }
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
            await context.SaveChangesAsync();
            Console.WriteLine($"[SeedData] Usuario administrador '{email}' creado exitosamente.");
        }
    }
}