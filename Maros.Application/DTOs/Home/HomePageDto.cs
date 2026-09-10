using Maros.Application.DTOs.Banners;
using Maros.Application.DTOs.Collections;
using Maros.Application.DTOs.Common;
using Maros.Application.DTOs.Seasons;
using Maros.Application.DTOs.Settings;
using Maros.Application.DTOs.Testimonials;

namespace Maros.Application.DTOs.Home;

public record HomePageDto(
    SeasonPublicResponseDto? ActiveSeason,
    CollectionPublicResponseDto? ActiveCollection,
    List<ProductSummaryDto> FeaturedProducts,
    List<BannerPublicDto> Banners,
    List<TestimonialPublicDto> Testimonials,
    SiteSettingsPublicDto Settings
);