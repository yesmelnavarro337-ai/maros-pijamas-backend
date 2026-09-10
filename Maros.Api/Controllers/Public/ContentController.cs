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

    public ContentController(
        IBlogService blogService,
        ITestimonialService testimonialService,
        IFaqService faqService,
        IBannerService bannerService,
        ISiteSettingsService settingsService)
    {
        _blogService = blogService;
        _testimonialService = testimonialService;
        _faqService = faqService;
        _bannerService = bannerService;
        _settingsService = settingsService;
    }

    [HttpGet("blog")]
    public async Task<IActionResult> GetBlog() => Ok(await _blogService.GetPublicAsync());

    [HttpGet("testimonials")]
    public async Task<IActionResult> GetTestimonials() => Ok(await _testimonialService.GetPublicAsync());

    [HttpGet("faq")]
    public async Task<IActionResult> GetFaq() => Ok(await _faqService.GetPublicAsync());

    [HttpGet("banners")]
    public async Task<IActionResult> GetBanners([FromQuery] string? position)
    {
        var banners = await _bannerService.GetPublicAsync(position);
        return Ok(banners);
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _settingsService.GetPublicAsync();
        return Ok(settings);
    }
}