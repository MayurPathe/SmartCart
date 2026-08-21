using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Infrastructure.Persistence;
using SmartCart.Identity.Infrastructure.Persistence.Repositories;
using SmartCart.Identity.Infrastructure.Security;

namespace SmartCart.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDatabase");

        services.AddDbContext<IdentityDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        services.Configure<JwtSettings>(
            configuration.GetSection(
                JwtSettings.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<
            IRefreshTokenRepository,
            RefreshTokenRepository>();

        services.AddScoped<
            ILoginAuditRepository,
            LoginAuditRepository>();

        services.AddScoped<
            IUnitOfWork,
            UnitOfWork>();

        services.AddScoped<
            IPasswordService,
            PasswordService>();

        services.AddScoped<
            IJwtTokenService,
            JwtTokenService>();

        return services;
    }
}
