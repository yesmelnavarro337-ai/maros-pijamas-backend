using Maros.Domain.Common;
using Maros.Domain.Enums;

namespace Maros.Domain.Entities;

public class Style : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? HexCode { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Línea de producto (adulto o infantil) a la que pertenece el estilo.</summary>
    public StyleLine Line { get; set; } = StyleLine.Adulto;

    public ICollection<ProductStyle> ProductStyles { get; set; } = new List<ProductStyle>();
}