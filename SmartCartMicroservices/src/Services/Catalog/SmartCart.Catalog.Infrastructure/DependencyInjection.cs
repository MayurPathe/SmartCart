using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCart.Catalog.Application.Interfaces;
using SmartCart.Catalog.Infrastructure.Cache;
using SmartCart.Catalog.Infrastructure.Persistence;
using SmartCart.Catalog.Infrastructure.Repositories;
using StackExchange.Redis;

namespace SmartCart.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString(
                    "CatalogDatabase"));
        });

        services.AddScoped<ICategoryRepository,
            CategoryRepository>();

        services.AddScoped<IProductRepository,
            ProductRepository>();

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(
                configuration["Redis:ConnectionString"]!));

        services.AddScoped<ICacheService,
            RedisCacheService>();

        return services;
    }
}
