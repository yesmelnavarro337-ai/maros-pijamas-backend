using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class CustomizationOption : BaseEntity
{
    public CustomizationCatalogType CatalogType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string? ColorHex { get; set; }
    public decimal? PriceModifier { get; set; }
    public bool Active { get; set; } = true;

    /// <summary>
    /// Opciones (telas, colores, estampados, bordados, tallas) asignadas a este
    /// registro cuando representa un modelo del personalizador.
    /// </summary>
    public ICollection<CustomizationOptionAssignment> AssignedOptions { get; set; } = new List<CustomizationOptionAssignment>();
}
