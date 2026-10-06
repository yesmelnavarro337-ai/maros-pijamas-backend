namespace Maros.Domain.Enums;

/// <summary>
/// Línea de producto a la que pertenece un estilo. Permite separar el catálogo
/// de adultos del infantil para que maros-admin ofrezca sólo los estilos que
/// corresponden a la categoría del producto.
/// </summary>
public enum StyleLine
{
    /// <summary>Línea de adultos (valores por defecto heredados).</summary>
    Adulto = 0,

    /// <summary>Línea infantil, gestionada bajo la categoría "Niños y Bebes".</summary>
    Infantil = 1
}