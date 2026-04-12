using System.Reflection;
using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using SharedKernel.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly applicationAssembly = typeof(DependencyInjection).Assembly;

        services.Scan(scan => scan.FromAssemblies(applicationAssembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<IScopedService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

        // Register FluentValidation validators (auto-discovered from this assembly)
        services.AddValidatorsFromAssembly(applicationAssembly, includeInternalTypes: true);

        services.AddScoped(typeof(ICommandBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(ICommandBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(CommandExecutor<,>));

        return services;
    }
}
