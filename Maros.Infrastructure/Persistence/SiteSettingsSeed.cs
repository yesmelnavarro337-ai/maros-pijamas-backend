using System.Text.Json;
using Maros.Domain.Entities;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence;

public static class SiteSettingsSeed
{
    private static readonly List<object> DefaultHomeSections =
    [
        new { id = "hero", label = "Hero principal", enabled = true, order = 1 },
        new { id = "active-season", label = "Colección / Temporada activa", enabled = true, order = 2 },
        new { id = "featured-products", label = "Productos destacados", enabled = true, order = 3 },
        new { id = "testimonials", label = "Testimonios", enabled = true, order = 4 },
        new { id = "blog", label = "Últimos artículos del blog", enabled = false, order = 5 },
        new { id = "newsletter", label = "Suscripción por correo", enabled = false, order = 6 },
    ];

    public static async Task SeedDefaultAsync(MarosDbContext context)
    {
        if (await context.SiteSettings.AnyAsync()) return;

        context.SiteSettings.Add(new SiteSettings
        {
            SiteName = "Maro's Pijamas",
            Description = "Pijamas hechas a mano con los mejores materiales",
            Currency = "COP - Peso Colombiano",
            Timezone = "UTC-05:00 Bogotá",
            Language = "Español",
            WhatsappNumber = "573013169974",
            WhatsappDefaultMessage = "¡Hola! Me interesa hacer un pedido de pijamas personalizadas.",
            Address = "Mz 3 Casa 98 Urb. Doña Clara, Valledupar",
            BusinessHours = "Lunes a Sábado · 8:00 a.m. – 6:00 p.m.",
            EmailFromName = "Maro's Pijamas",
            EmailFromAddress = string.Empty,
            BackupFrequency = "semanal",
            SessionTimeoutMinutes = 60,
            HomeSectionsJson = JsonSerializer.Serialize(DefaultHomeSections),
        });

        await context.SaveChangesAsync();
    }
}