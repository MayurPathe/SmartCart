using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartCart.Identity.Application.Common.Behaviors;
namespace SmartCart.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly =
            typeof(DependencyInjection).Assembly;

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(
                assembly);
        });

        services.AddValidatorsFromAssembly(
            assembly);

        //services.AddAutoMapper(assembly);
        services.AddAutoMapper(config => { }, assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}