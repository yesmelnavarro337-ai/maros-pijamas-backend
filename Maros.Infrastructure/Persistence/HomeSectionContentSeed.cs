using System.Text.Json;
using Maros.Domain.Entities;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence;

public static class HomeSectionContentSeed
{
    public static async Task SeedDefaultAsync(MarosDbContext context)
    {
        // Semilla idempotente por SectionKey: si una fila ya existe (creada desde el
        // admin o por un arranque previo) no se toca, para no pisar lo que ya editó
        // el equipo. Solo se crean las secciones que falten.
        await UpsertIfMissingAsync(context, "personalize", new HomeSectionContent
        {
            SectionKey = "personalize",
            SectionTitle = "Personaliza tu pijama",
            SectionSubtitle = "Elige cada detalle y crea algo único.",
            CtaText = "Diseñar mi pijama",
            CtaLink = "/personaliza",
            MainImageUrl = null,
            MainImageAlt = "Diseña tu pijama personalizada",
        });

        await UpsertIfMissingAsync(context, "featured-collection", new HomeSectionContent
        {
            SectionKey = "featured-collection",
            SectionTitle = "Colección destacada",
            SectionSubtitle = "Diseños que inspiran momentos especiales.",
            Eyebrow = "CAMPAÑA DESTACADA",
            CtaText = "Ver colección",
        });

        await UpsertIfMissingAsync(context, "brand-promise", new HomeSectionContent
        {
            SectionKey = "brand-promise",
            Eyebrow = "NUESTRA PROMESA",
            SectionTitle = "Hecho con intención.",
            BodyText = "Telas de alta calidad, diseños únicos y cada detalle pensado para acompañarte en tus momentos de descanso y unión familiar.",
            TagsJson = JsonSerializer.Serialize(new[]
            {
                new { label = "Telas premium", icon = "tag" },
                new { label = "Hecho a mano", icon = "scissors" },
                new { label = "Diseños únicos", icon = "gem" },
                new { label = "Calidad garantizada", icon = "shield-check" },
                new { label = "Pago al recibir", icon = "package-check" },
            }),
        });
    }

    private static async Task UpsertIfMissingAsync(MarosDbContext context, string sectionKey, HomeSectionContent content)
    {
        var exists = await context.HomeSectionContents
            .AnyAsync(s => s.SectionKey == sectionKey);

        if (exists) return;

        context.HomeSectionContents.Add(content);
        await context.SaveChangesAsync();
    }
}
