using Maros.Application.Common;
using Maros.Application.DTOs.Collections;
using Maros.Application.DTOs.Home;
using Maros.Application.DTOs.Seasons;
using Maros.Application.Interfaces;

namespace Maros.Application.Services;

public class HomeService : IHomeService
{
    private readonly ISeasonService _seasonService;
    private readonly ICollectionService _collectionService;
    private readonly IProductService _productService;
    private readonly IBannerService _bannerService;
    private readonly ITestimonialService _testimonialService;
    private readonly ISiteSettingsService _settingsService;

    public HomeService(
        ISeasonService seasonService,
        ICollectionService collectionService,
        IProductService productService,
        IBannerService bannerService,
        ITestimonialService testimonialService,
        ISiteSettingsService settingsService)
    {
        _seasonService = seasonService;
        _collectionService = collectionService;
        _productService = productService;
        _bannerService = bannerService;
        _testimonialService = testimonialService;
        _settingsService = settingsService;
    }

    public async Task<HomePageDto> GetHomeAsync()
    {
        // "No hay temporada/colección activa configurada" es un estado de negocio
        // válido (aún no hay campaña corriendo), no un error del sistema —
        // por eso se captura aquí y el home simplemente omite esa sección,
        // en vez de que toda la página falle por un 404 esperado.
        var activeSeason = await TryGetAsync(() => _seasonService.GetActivePublicAsync());
        var activeCollection = await TryGetAsync<CollectionPublicResponseDto?>(async () =>
            await _collectionService.GetActivePublicAsync());

        var featuredProducts = await _productService.GetFeaturedHomeAsync();
        var banners = await _bannerService.GetPublicAsync(position: null);
        var testimonials = (await _testimonialService.GetPublicAsync()).Take(5).ToList();
        var settings = await _settingsService.GetPublicAsync();

        return new HomePageDto(
            activeSeason,
            activeCollection,
            featuredProducts,
            banners,
            testimonials,
            settings
        );
    }

    private static async Task<T?> TryGetAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (AppException)
        {
            return default;
        }
    }
}