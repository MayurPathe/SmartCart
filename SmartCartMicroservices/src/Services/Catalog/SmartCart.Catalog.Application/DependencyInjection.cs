using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;

namespace SmartCart.Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(
        this IServiceCollection services)
    {
        var assembly =
            Assembly.GetExecutingAssembly();

        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(assembly));

        services.AddAutoMapper(assembly);

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
