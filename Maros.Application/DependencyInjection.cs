using Maros.Application.Interfaces;
using Maros.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Maros.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<ISeasonService, SeasonService>();
        services.AddScoped<ICustomizationOptionService, CustomizationOptionService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IQuotationService, QuotationService>();
        services.AddScoped<IWhatsAppService, WhatsAppService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<ITestimonialService, TestimonialService>();
        services.AddScoped<IFaqService, FaqService>();
        services.AddScoped<IBannerService, BannerService>();
        services.AddScoped<IPageHeaderService, PageHeaderService>();
        services.AddScoped<ISiteSettingsService, SiteSettingsService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IContactMessageService, ContactMessageService>();
        services.AddScoped<IInvitationService, InvitationService>();
        return services;
    }
}