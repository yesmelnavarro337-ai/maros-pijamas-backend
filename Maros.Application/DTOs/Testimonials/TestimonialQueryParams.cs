namespace Maros.Application.DTOs.Testimonials;

public class TestimonialQueryParams
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
