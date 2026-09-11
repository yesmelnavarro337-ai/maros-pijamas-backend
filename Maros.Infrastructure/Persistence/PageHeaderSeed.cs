using Maros.Domain.Entities;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence;

public static class PageHeaderSeed
{
    public static async Task SeedDefaultAsync(MarosDbContext context)
    {
        if (await context.PageHeaders.AnyAsync()) return;

        context.PageHeaders.AddRange(
            new PageHeader
            {
                PageKey = "collections",
                Title = "Nuestras Colecciones",
                Subtitle = "Ediciones especiales diseñadas con amor y confort para cada temporada.",
            },
            new PageHeader
            {
                PageKey = "blog",
                Title = "Blog",
                Subtitle = "Consejos, inspiración y todo sobre pijamas personalizadas.",
            },
            new PageHeader
            {
                PageKey = "gallery",
                Title = "Galería",
                Subtitle = "Momentos especiales con Maro's Pijamas.",
            }
        );

        await context.SaveChangesAsync();
    }
}