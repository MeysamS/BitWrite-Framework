using Bw.Core.Reflection;
using Bw.EventSourcing.Projections;
using Bw.Extensions.Microsoft.DependencyInjection;
using Bw.Persistence.EventStore.Abstraction;
using Bw.Persistence.EventStore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bw.Persistence.EventStore.Extensions;

public static class EventStoreRegistrationExtentions
{
    public static IServiceCollection AddInMemoryEventStore(this IServiceCollection services)
    {
        return AddEventStore<InMemoryEventStore>(services, ServiceLifetime.Singleton);

    }


    public static IServiceCollection AddEventStore<TEventStore>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
    ) where TEventStore : class, IEventStore
    {
        services.Add<IAggregateStore, AggregateStore>(serviceLifetime);
        return services
            .Add<TEventStore, TEventStore>(serviceLifetime)
            .Add<IEventStore>(sp => sp.GetRequiredService<TEventStore>(), serviceLifetime);
    }

    public static IServiceCollection AddReadProjections(
    this IServiceCollection services,
       params Assembly[] scanAssemblies
   )
    {
        services.AddSingleton<IReadProjectionPublisher, ReadProjectionPublisher>();

        // Assemblies are lazy loaded so using AppDomain.GetAssemblies is not reliable.
        var assemblies = scanAssemblies.Any()
            ? scanAssemblies
            : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).ToArray();

        RegisterProjections(services, assemblies!);

        return services;
    }

    private static void RegisterProjections(IServiceCollection services, Assembly[] assembliesToScan)
    {
        services.Scan(
            scan =>
                scan.FromAssemblies(assembliesToScan)
                    .AddClasses(classes => classes.AssignableTo<IHaveReadProjection>()) // Filter classes
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
        );
    }
}