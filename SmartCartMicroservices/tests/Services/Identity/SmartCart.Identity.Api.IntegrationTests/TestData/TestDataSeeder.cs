using Microsoft.EntityFrameworkCore;
using SmartCart.Identity.Domain.Constants;
using SmartCart.Identity.Domain.Entities;
using SmartCart.Identity.Infrastructure.Persistence;

namespace SmartCart.Identity.Api.IntegrationTests.TestData;

public static class TestDataSeeder
{
    public static async Task SeedAsync(
        IdentityDbContext dbContext)
    {
        //await dbContext.Database.EnsureCreatedAsync();

        // Check if User role already exists
        var userRole =
            await dbContext.Roles
                .FirstOrDefaultAsync(
                    x => x.Name ==
                         RoleNames.User.ToUpperInvariant());

        if (userRole == null)
        {
            userRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = RoleNames.User.ToUpperInvariant()
            };

            dbContext.Roles.Add(userRole);

            await dbContext.SaveChangesAsync();
        }
    }
}