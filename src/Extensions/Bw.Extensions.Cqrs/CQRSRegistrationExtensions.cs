using Bw.Core.Cqrs.Commands;
using Bw.Core.Cqrs.Event;
using Bw.Core.Cqrs.Query;
using Bw.Core.Domain.Event.Internal;
using Bw.Core.Scheduling;
using Bw.Cqrs.Command;
using Bw.Cqrs.Event;
using Bw.Cqrs.Query;
using Bw.Domain.Event;
using Bw.Domain.Extensions;
using Bw.Extensions.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bw.Extensions.Cqrs;

public static class CQRSRegistrationExtensions
{
    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Assembly[]? assemblies = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
        params Type[] pipelines)
    {

        services.AddMediatR(
            assemblies ?? new[] { Assembly.GetCallingAssembly() },
            x =>
            {
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Transient:
                        x.AsTransient();
                        break;
                    case ServiceLifetime.Scoped:
                        x.AsScoped();
                        break;
                    case ServiceLifetime.Singleton:
                        x.AsSingleton();
                        break;
                }
            });
        foreach (var pipeline in pipelines)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), pipeline);
        }

        services.Add<ICommandProcessor, CommandProcessor>(serviceLifetime)
          .Add<IQueryProcessor, QueryProcessor>(serviceLifetime)
          .Add<IEventProcessor, EventProcessor>(serviceLifetime)
          .Add<ICommandScheduler, PersistanceCommandScheduler>(serviceLifetime)
          .Add<IDomainEventPublisher, DomainEventPublisher>(serviceLifetime)
          .Add<IDomainNotificationEventPublisher, DomainNotificationEventPublisher>(serviceLifetime);
        services.AddDomain();
        // Todo
        //services.AddScoped<IDomainEventsAccessor, NullDomainEventsAccessor>();

        return services;
    }
}
