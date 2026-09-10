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
        if (await context.Users.AnyAsync()) return;

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            // No hay credenciales configuradas — no sembramos nada.
            // Esto evita crear un admin con contraseña vacía o predecible por accidente.
            return;
        }

        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            Name = "Administradora",
            Email = email,
            Role = UserRole.Administrador,
            Status = UserStatus.Activo,
        };
        admin.PasswordHash = hasher.HashPassword(admin, password);

        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}