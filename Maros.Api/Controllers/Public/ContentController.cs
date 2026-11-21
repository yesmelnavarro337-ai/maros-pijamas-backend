using Maros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/public")]
[AllowAnonymous]
public class ContentController : ControllerBase
{
    private readonly IBlogService _blogService;
    private readonly ITestimonialService _testimonialService;
    private readonly IFaqService _faqService;
    private readonly IBannerService _bannerService;
    private readonly ISiteSettingsService _settingsService;
    private readonly ICacheService _cacheService;

    public ContentController(
        IBlogService blogService,
        ITestimonialService testimonialService,
        IFaqService faqService,
        IBannerService bannerService,
        ISiteSettingsService settingsService,
        ICacheService cacheService)
    {
        _blogService = blogService;
        _testimonialService = testimonialService;
        _faqService = faqService;
        _bannerService = bannerService;
        _settingsService = settingsService;
        _cacheService = cacheService;
    }

    [HttpGet("blog")]
    public async Task<IActionResult> GetBlog()
    {
        var blog = await _cacheService.GetOrCreateAsync(
            "public:blog",
            () => _blogService.GetPublicAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(blog);
    }

    [HttpGet("testimonials")]
    public async Task<IActionResult> GetTestimonials()
    {
        var testimonials = await _cacheService.GetOrCreateAsync(
            "public:testimonials",
            () => _testimonialService.GetPublicAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(testimonials);
    }

    [HttpGet("faq")]
    public async Task<IActionResult> GetFaq()
    {
        var faq = await _cacheService.GetOrCreateAsync(
            "public:faq",
            () => _faqService.GetPublicAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(faq);
    }

    [HttpGet("banners")]
    public async Task<IActionResult> GetBanners([FromQuery] string? position)
    {
        var key = $"public:banners:{position ?? "all"}";
        var banners = await _cacheService.GetOrCreateAsync(
            key,
            () => _bannerService.GetPublicAsync(position),
            TimeSpan.FromMinutes(5));
        return Ok(banners);
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _cacheService.GetOrCreateAsync(
            "public:settings",
            () => _settingsService.GetPublicAsync(),
            TimeSpan.FromMinutes(5));
        return Ok(settings);
    }
}