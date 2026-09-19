using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Maros.Infrastructure.Persistence.Context;

public class MarosDbContextFactory : IDesignTimeDbContextFactory<MarosDbContext>
{
    public MarosDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("MAROS_MIGRATIONS_CONNECTION")
            ?? "Host=localhost;Database=maros_migrations;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<MarosDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new MarosDbContext(options);
    }
}
