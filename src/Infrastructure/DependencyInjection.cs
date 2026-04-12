using System.Reflection;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString) =>
        services
            .AddServices()
            .AddPersistence(connectionString);

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        Assembly infrastructureAssembly = typeof(DependencyInjection).Assembly;

        services.Scan(scan => scan.FromAssemblies(infrastructureAssembly)
            .AddClasses(classes => classes.AssignableTo<ITransientService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<IScopedService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>(), publicOnly: false)
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<DemoDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
