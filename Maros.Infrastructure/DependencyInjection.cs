using Maros.Application.Interfaces;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Maros.Infrastructure.Persistence.Repositories;
using Maros.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Maros.Infrastructure.ExternalServices;

namespace Maros.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<MarosDbContext>(options =>
            options.UseNpgsql(connectionString));

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
        services.AddScoped<ISiteSettingsRepository, SiteSettingsRepository>();
        services.AddScoped<IPaginationService, PaginationService>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        return services;
    }
}