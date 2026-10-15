using Maros.Application.Interfaces;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Maros.Infrastructure.Persistence.Repositories;
using Maros.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Maros.Infrastructure.ExternalServices; // Incluye CloudinaryImageStorageService, SmtpEmailService y (desactivado) GeminiClient
using Npgsql;

namespace Maros.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<MarosDbContext>(options =>
            options.UseNpgsql(ApplyConnectionPoolLimits(connectionString)));

        // ─── FUTURA IMPLEMENTACIÓN: IA Gemini ─────────────────────────────────
        // Cliente HTTP y servicio de Gemini para el asistente de personalización.
        // Para reactivar: descomentar estas líneas, restaurar IGeminiClient,
        // GeminiClient, ICustomizationAssistantService y CustomizationAssistantService.
        // services.AddHttpClient("GeminiClient", client =>
        // {
        //     client.Timeout = TimeSpan.FromSeconds(30);
        // });
        // ─────────────────────────────────────────────────────────────────────

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<IStyleRepository, StyleRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<ICustomizationOptionRepository, CustomizationOptionRepository>();
        // [FUTURA IMPLEMENTACIÓN: IA Gemini] Descomentar para activar el asistente de personalización
        // services.AddScoped<IGeminiClient, GeminiClient>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IQuotationRepository, QuotationRepository>();
        services.AddScoped<ISiteSettingsRepository, SiteSettingsRepository>();
        services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
        services.AddScoped<IGalleryImageRepository, GalleryImageRepository>();
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        services.AddScoped<ITestimonialRepository, TestimonialRepository>();
        services.AddScoped<IFaqRepository, FaqRepository>();
        services.AddScoped<IBannerRepository, BannerRepository>();
        services.AddScoped<IPageHeaderRepository, PageHeaderRepository>();
        services.AddScoped<IHomeSectionContentRepository, HomeSectionContentRepository>();
        services.AddScoped<ISiteSettingsRepository, SiteSettingsRepository>();
        services.AddScoped<IPaginationService, PaginationService>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        return services;
    }

    /// <summary>
    /// Limita el pool local de Npgsql por debajo del que ofrece el pooler de
    /// Supabase (pool_size: 15 en session mode). Mantiene un límite conservador de
    /// conexiones y un tiempo de inactividad corto para no agotar los slots del pooler
    /// cuando conviven el entorno local, Render y herramientas externas.
    /// </summary>
    private static string ApplyConnectionPoolLimits(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'DefaultConnection'. "
                + "Configúrala en user-secrets, appsettings o como variable de entorno.");
        }

        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        // Límite estricto de conexiones simultáneas por proceso
        builder.MaxPoolSize = MaxPoolSize;
        builder.MinPoolSize = MinPoolSize;

        // Cierra conexiones inactivas rápidamente (10s) para liberar inmediatamente los slots de Supabase
        builder.ConnectionIdleLifetime = ConnectionIdleLifetimeSeconds;

        // Recicla las conexiones cada 60s para evitar conexiones colgadas
        builder.ConnectionLifetime = ConnectionLifetimeSeconds;

        return builder.ConnectionString;
    }

    // 4 conexiones cubren con holgura el tráfico de la API y evitan saturar el pool de 15 de Supabase
    private const int MaxPoolSize = 4;
    private const int MinPoolSize = 0;
    private const int ConnectionIdleLifetimeSeconds = 10;
    private const int ConnectionLifetimeSeconds = 60;
}