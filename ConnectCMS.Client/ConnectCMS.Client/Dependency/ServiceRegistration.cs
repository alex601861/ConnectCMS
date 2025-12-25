using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Dependency;

public static class ServiceRegistration
{
    public static IServiceCollection AddDependencyServices(this IServiceCollection services) =>
        services
            .AddServices(typeof(ITransientService), ServiceLifetime.Transient)
            .AddServices(typeof(ISingletonService), ServiceLifetime.Singleton)
            .AddServices(typeof(IScopedService), ServiceLifetime.Scoped);

    private static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
    {
        var interfaceTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => interfaceType.IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false })
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i != typeof(IScopedService) && i != typeof(ISingletonService) && i != typeof(ITransientService))
                    .Select(i => new
                    {
                        Service = i,
                        Implementation = t
                    }))
                .Where(t => interfaceType.IsAssignableFrom(t.Service));

        foreach (var type in interfaceTypes)
        {
            services.AddService(type.Service, type.Implementation, lifetime);
        }

        return services;
    }

    private static IServiceCollection AddService(this IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime) =>
        lifetime switch
        {
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifetime service injection", nameof(lifetime))
        };
}