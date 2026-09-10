using Maros.Domain.Entities;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence;

public static class SiteSettingsSeed
{
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
            WhatsappNumber = string.Empty,
            WhatsappDefaultMessage = "¡Hola! Me interesa hacer un pedido de pijamas personalizadas.",
            EmailFromName = "Maro's Pijamas",
            EmailFromAddress = string.Empty,
            BackupFrequency = "semanal",
            SessionTimeoutMinutes = 60,
        });

        await context.SaveChangesAsync();
    }
}