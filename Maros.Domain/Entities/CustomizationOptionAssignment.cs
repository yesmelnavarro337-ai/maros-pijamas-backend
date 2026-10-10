namespace Maros.Domain.Entities;

/// <summary>
/// Relación modelo → opción. Asocia un modelo del personalizador
/// (<see cref="CustomizationOption"/> de tipo Modelo) con las opciones
/// (telas, colores, estampados, bordados, tallas) que lo componen.
/// </summary>
public class CustomizationOptionAssignment
{
    public Guid ModelId { get; set; }
    public CustomizationOption Model { get; set; } = null!;

    public Guid OptionId { get; set; }
    public CustomizationOption Option { get; set; } = null!;
}
