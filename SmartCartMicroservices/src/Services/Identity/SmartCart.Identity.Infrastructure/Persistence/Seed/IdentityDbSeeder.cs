using Microsoft.EntityFrameworkCore;
using SmartCart.Identity.Domain.Constants;
using SmartCart.Identity.Domain.Entities;

namespace SmartCart.Identity.Infrastructure.Persistence.Seed;

public static class IdentityDbSeeder
{
    public static async Task SeedAsync(
        IdentityDbContext dbContext)
    {
        var roles = new[]
        {
            RoleNames.Admin,
            RoleNames.User
        };

        foreach (var roleName in roles)
        {
            var normalizedName =
                roleName.ToUpperInvariant();

            var exists =
                await dbContext.Roles
                    .AnyAsync(
                        x =>
                            x.NormalizedName ==
                            normalizedName);

            if (!exists)
            {
                await dbContext.Roles.AddAsync(
                    new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName,
                        NormalizedName = normalizedName,
                        CreatedAt = DateTime.UtcNow
                    });
            }
        }

        await dbContext.SaveChangesAsync();
    }
}