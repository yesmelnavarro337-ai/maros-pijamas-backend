using Maros.Domain.Entities;

namespace Maros.Domain.Interfaces;

public interface ISiteSettingsRepository
{
    Task<SiteSettings?> GetAsync();
    Task AddAsync(SiteSettings settings);
    Task SaveChangesAsync();

    /// <summary>
    /// Solo lectura del contador de visitas (sin mutar la BD).
    /// </summary>
    Task<long> GetVisitorsCountAsync();

    /// <summary>
    /// Incremento atómico en la base de datos y devuelve el valor resultante.
    /// </summary>
    Task<long> IncrementVisitorsAsync();
}