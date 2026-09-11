using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence.Context;

public class MarosDbContext : DbContext
{
    public MarosDbContext(DbContextOptions<MarosDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<ProductCollection> ProductCollections => Set<ProductCollection>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<SeasonFeaturedProduct> SeasonFeaturedProducts => Set<SeasonFeaturedProduct>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
    public DbSet<QuotationItemOption> QuotationItemOptions => Set<QuotationItemOption>();
    public DbSet<QuotationReferenceImage> QuotationReferenceImages => Set<QuotationReferenceImage>();
    public DbSet<CustomizationOption> CustomizationOptions => Set<CustomizationOption>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<PageHeader> PageHeaders => Set<PageHeader>();
    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarosDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}