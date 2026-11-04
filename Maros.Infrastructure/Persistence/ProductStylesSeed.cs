using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence;

public static class ProductStylesSeed
{
    public static readonly (Guid Id, string Name, string Slug, StyleLine Line, int DisplayOrder) ShortTirasMujer = (
        Guid.Parse("11111111-1111-1111-1111-111111111110"),
        "Short - Tiras (Mujer)",
        "short-tiras-mujer",
        StyleLine.Adulto,
        10
    );

    public static readonly (Guid Id, string Name, string Slug, StyleLine Line, int DisplayOrder) ShortCamisetaInfantil = (
        Guid.Parse("11111111-1111-1111-1111-111111111204"),
        "Short - Camiseta (Infantil)",
        "short-camiseta-infantil",
        StyleLine.Infantil,
        11
    );

    public static readonly (Guid Id, string Name, string Slug, StyleLine Line, int DisplayOrder) PantalonCamisetaInfantil = (
        Guid.Parse("11111111-1111-1111-1111-111111111205"),
        "Pantalón - Camiseta (Infantil)",
        "pantalon-camiseta-infantil",
        StyleLine.Infantil,
        12
    );

    /// <summary>
    /// Recorre todos los productos activos y les asigna automáticamente los 3 nuevos estilos
    /// si aún no los tienen registrados, evitando duplicados.
    /// </summary>
    public static async Task SeedNewStylesAsync(MarosDbContext db)
    {
        var definitions = new[] { ShortTirasMujer, ShortCamisetaInfantil, PantalonCamisetaInfantil };
        var resolvedStyles = new List<Style>();

        foreach (var def in definitions)
        {
            var existing = await db.Styles.FirstOrDefaultAsync(s => s.Id == def.Id || s.Slug == def.Slug || s.Name == def.Name);
            if (existing == null)
            {
                existing = new Style
                {
                    Id = def.Id,
                    Name = def.Name,
                    Slug = def.Slug,
                    Line = def.Line,
                    DisplayOrder = def.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                db.Styles.Add(existing);
                await db.SaveChangesAsync();
            }
            resolvedStyles.Add(existing);
        }

        // Obtener todos los productos activos con sus estilos actuales
        var activeProducts = await db.Products
            .Include(p => p.ProductStyles)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Where(p => !p.IsDeleted && p.Status == ProductStatus.Activo)
            .ToListAsync();

        int assignedCount = 0;

        foreach (var product in activeProducts)
        {
            var isInfantilOnly = product.ProductCategories.Count > 0 &&
                                 product.ProductCategories.All(pc => pc.Category != null &&
                                    (pc.Category.Slug.Contains("nino") || pc.Category.Slug.Contains("infantil") || pc.Category.Slug.Contains("bebe")));

            var isAdultOnly = product.ProductCategories.Count > 0 &&
                              product.ProductCategories.All(pc => pc.Category != null &&
                                    !pc.Category.Slug.Contains("nino") && !pc.Category.Slug.Contains("infantil") && !pc.Category.Slug.Contains("bebe") && !pc.Category.Slug.Contains("familia"));

            foreach (var style in resolvedStyles)
            {
                // Validación de compatibilidad:
                // Si el producto es estrictamente infantil, solo se asignan estilos infantiles.
                // Si es estrictamente de adulto, solo estilos de adulto.
                // Si admite ambas o no tiene categorías restrictivas, se le asignan los 3 estilos.
                if (isInfantilOnly && style.Line == StyleLine.Adulto) continue;
                if (isAdultOnly && style.Line == StyleLine.Infantil) continue;

                // Evitar duplicados
                bool alreadyHasStyle = product.ProductStyles.Any(ps => ps.StyleId == style.Id);
                if (!alreadyHasStyle)
                {
                    product.ProductStyles.Add(new ProductStyle
                    {
                        ProductId = product.Id,
                        StyleId = style.Id
                    });
                    assignedCount++;
                }
            }
        }

        if (assignedCount > 0)
        {
            await db.SaveChangesAsync();
            Console.WriteLine($"[ProductStylesSeed] Se asignaron {assignedCount} asociaciones de nuevos estilos a productos activos exitosamente.");
        }
    }
}

