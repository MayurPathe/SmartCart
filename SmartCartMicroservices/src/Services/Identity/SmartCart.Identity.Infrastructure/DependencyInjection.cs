using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCart.Identity.Application.Interfaces;
using SmartCart.Identity.Infrastructure.Data;
using SmartCart.Identity.Infrastructure.Repositories;
using SmartCart.Identity.Infrastructure.Services;

namespace SmartCart.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = configuration["Jwt:Issuer"] ?? string.Empty;
            options.Audience = configuration["Jwt:Audience"] ?? string.Empty;
            options.SecretKey = configuration["Jwt:SecretKey"] ?? string.Empty;

            var expiryValue = configuration["Jwt:ExpiryMinutes"];
            options.ExpiryMinutes = int.TryParse(expiryValue, out var expiryMinutes)
                ? expiryMinutes
                : 60;
        });

        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("IdentityDb"));
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
