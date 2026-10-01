using Maros.Application.Interfaces;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Maros.Infrastructure.Persistence.Repositories;
using Maros.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Maros.Infrastructure.ExternalServices;
using Npgsql;

namespace Maros.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<MarosDbContext>(options =>
            options.UseNpgsql(ApplyConnectionPoolLimits(connectionString)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<ICustomizationOptionRepository, CustomizationOptionRepository>();
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
    /// Supabase (pool_size: 15 en session mode). Sin esto, Npgsql abre hasta 100
    /// conexiones por proceso y agota los slots del pooler, provocando
    /// "max clients reached in session mode" al arrancar.
    ///
    /// Se aplica sobre el connection string para que funcione con cualquier origen
    /// de configuración (user-secrets, appsettings o variables de entorno en Render)
    /// sin duplicar la cadena en más de un archivo.
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

        if (builder.MaxPoolSize <= 0 || builder.MaxPoolSize > MaxPoolSize)
        {
            builder.MaxPoolSize = MaxPoolSize;
        }

        if (builder.MinPoolSize < MinPoolSize)
        {
            builder.MinPoolSize = MinPoolSize;
        }

        // Recicla las conexiones antes de que Supabase o un balanceador las corten,
        // evitando fallos transitorios por conexiones zombis.
        if (builder.ConnectionLifetime <= 0)
        {
            builder.ConnectionLifetime = ConnectionLifetimeSeconds;
        }

        return builder.ConnectionString;
    }

    // 10 conexiones cubren la carga de esta API con holgura y dejan libre la mitad
    // del pool del pooler para otros procesos (CLI de migraciones, diagnostics).
    private const int MaxPoolSize = 10;
    private const int MinPoolSize = 0;

    // Npgsql expone ConnectionLifetime en segundos, no como TimeSpan.
    private const int ConnectionLifetimeSeconds = 300;
}