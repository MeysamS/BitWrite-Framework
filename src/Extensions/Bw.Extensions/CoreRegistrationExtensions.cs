using Bw.Core.Domain.Event;
using Bw.Core.Domain.Event.Mapper;
using Bw.Core.Messaging;
using Bw.Core.Messaging.Bus;
using Bw.Core.Reflection;
using Bw.Core.Types;
using Bw.Domain.Event;
using Bw.Extensions.Microsoft.DependencyInjection;
using Bw.Messaging.BackgroundServices;
using Bw.Messaging.Persistence;
using Bw.Messaging.Serializations;
using Bw.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrutor;
using System.Reflection;

namespace Bw.Extensions;

public static class CoreRegistrationExtensions
{
    public static IServiceCollection AddCore(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] scanAssemblies)
    {
        var systemInfo = MachineInstanceInfo.New();
        var assemblies = scanAssemblies.Any()
        ? scanAssemblies
        : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).Distinct().ToArray();

        services.AddSingleton<IMachineInstanceInfo>(systemInfo);
        services.AddSingleton(systemInfo);
        //services.AddSingleton<IExclusiveLock, ExclusiveLock>();


        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddScoped<IMessagePersistenceRepository, NullMessagePersistenceRepository>();

        services.AddHttpContextAccessor();

        AddDefaultSerializer(services);


        services.AddMessagingCore(configuration, assemblies);


        RegisterEventMappers(services, assemblies);


        return services;

    }

    private static void AddMessagingCore(
       this IServiceCollection services,
       IConfiguration configuration,
       Assembly[] scanAssemblies,
       ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
    {
        services.AddScoped<IBus, UnitBus>();
        AddMessageingMediator(services, serviceLifetime, scanAssemblies);
        AddPersistenceMessage(services, configuration);
    }
    private static void AddMessageingMediator(
        IServiceCollection services,
        ServiceLifetime serviceLifetime,
        Assembly[] scanAssemblies)
    {
        services.Scan(
            scan =>
                scan.FromAssemblies(scanAssemblies)
                    .AddClasses(classes => classes.AssignableTo(typeof(IMessageHandler<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Append)
                    .AsClosedTypeOf(typeof(IMessageHandler<>))
                    .AsSelf()
                    .WithLifetime(serviceLifetime));
    }

    public static void AddDefaultHostedMessageBackgroundService(IServiceCollection services)
    {
        services.AddHostedService<MessagePersistenceBackgroundService>();
    }

    public static void AddHostedMessageBackgroundService<TBackGroundService>(IServiceCollection services)
        where TBackGroundService : BackgroundService
    {
        services.AddHostedService<TBackGroundService>();
    }


    private static void AddPersistenceMessage(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMessagePersistenceRepository, NullMessagePersistenceRepository>();
        services.AddScoped<IMessagePersistenceService, MessagePersistenceService>();
        services.AddOptions<MessagePersistenceOptions>()
                    .Bind(configuration.GetSection(nameof(MessagePersistenceOptions)))
                    .ValidateDataAnnotations();
    }


    private static void AddDefaultSerializer(
    IServiceCollection services,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.Add<ISerializer, DefaultSerializer>(lifetime);
        services.Add<IMessageSerializer, DefaultMessageSerializer>(lifetime);
    }

    private static void RegisterEventMappers(IServiceCollection services, Assembly[] scanAssemblies)
    {
        services.Scan(
            scan =>
                scan.FromAssemblies(scanAssemblies)
                .AddClasses(classess => classess.AssignableTo(typeof(IEventMapper)), false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventMapper)), false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IIDomainNotificationEventMapper)), false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
    }

}