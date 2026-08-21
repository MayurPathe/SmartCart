using Microsoft.Extensions.DependencyInjection;
using SmartCart.Identity.Infrastructure.Persistence;
using SmartCart.Identity.Api.IntegrationTests.TestData;

namespace SmartCart.Identity.Api.IntegrationTests.Auth;

public abstract class IntegrationTestBase
    : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly CustomWebApplicationFactory Factory;

    protected readonly HttpClient Client;

    protected IntegrationTestBase(
        CustomWebApplicationFactory factory)
    {
        Factory = factory;

        Client = factory.CreateClient();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope =
            Factory.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<IdentityDbContext>();

        TestDataSeeder
            .SeedAsync(dbContext)
            .GetAwaiter()
            .GetResult();
    }
}