using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartCart.Catalog.Infrastructure.Persistence;

public class CatalogDbContextFactory
    : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<CatalogDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=catalog_db;Username=postgres;Password=root");

        return new CatalogDbContext(
            optionsBuilder.Options);
    }
}