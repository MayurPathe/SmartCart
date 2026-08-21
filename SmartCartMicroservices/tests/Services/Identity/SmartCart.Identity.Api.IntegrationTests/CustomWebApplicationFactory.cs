using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartCart.Identity.Infrastructure.Persistence;

namespace SmartCart.Identity.Api.IntegrationTests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private const string TestConnectionString =
        "Host=localhost;" +
        "Port=5432;" +
        "Database=identity_test_db;" +
        "Username=postgres;" +
        "Password=root";


    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");


        builder.ConfigureServices(
            services =>
            {
                // Remove existing DbContext registration.
                services.RemoveAll<
                    DbContextOptions<IdentityDbContext>>();

                services.RemoveAll<
                    IdentityDbContext>();


                // Register PostgreSQL test database.
                services.AddDbContext<
                    IdentityDbContext>(
                    options =>
                    {
                        options.UseNpgsql(
                            TestConnectionString);
                    });
            });
    }
}