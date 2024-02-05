using Bw.Core.Messaging;
using Bw.Messaging.Event;
using Sic.Xamin.Domain.Abstraction.Event.Internal;
using Sic.Xamin.Domain.Abstraction.Messaging;
using Sic.Xamin.Reflection.Extensions;
using System.Reflection;

namespace Bw.DomainEvent;

public static class EventsExtensions
{
    public static IEnumerable<Type> GetHandledIntegrationEventTypes(this Assembly[] assemblies)
    {
        var messagehandlerTypes = typeof(IIntegrationEventHandler<>)
            .GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messagehandlerTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(
            x =>
                x.GetInterfaces().Any(i => i.IsGenericType) && x.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));
        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IIntegrationEvent)))
            {
                yield return messageType;
            }
        }
    }

    public static IEnumerable<Type> GetHandledDomainNotificationEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IDomainNotificationEventHandler<>)
            .GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(
                x =>
                    x.GetInterfaces().Any(i => i.IsGenericType)
                    && x.GetGenericTypeDefinition() == typeof(IDomainNotificationEventHandler<>)
            );

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IDomainNotificationEvent)))
            {
                yield return messageType;
            }
        }
    }

    public static IEnumerable<Type> GetHandledDomainEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IDomainEventHandler<>)
            .GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(
                x =>
                    x.GetInterfaces().Any(i => i.IsGenericType)
                    && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
            );

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IDomainEvent)))
            {
                yield return messageType;
            }
        }
    }

    public static IEnumerable<IDomainNotificationEvent> GetWrappedDomainNotificationEvents(
        this IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (
            IDomainEvent domainEvent in domainEvents.Where(
                x => typeof(IHaveNotificationEvent).IsAssignableFrom(x.GetType())
                )
        )
        {
            Type genericType = typeof(DomainNotificationEventWrapper<>).MakeGenericType(domainEvent.GetType());

            IDomainNotificationEvent? domainNotificationEvent = (IDomainNotificationEvent?)
                Activator.CreateInstance(genericType, domainEvent);

            if (domainNotificationEvent is not null)
                yield return domainNotificationEvent;
        }
    }

    public static IEnumerable<IIntegrationEvent> GetWrappedIntegrationEvents(
    this IEnumerable<IDomainEvent> domainEvents
)
    {
        foreach (
            IDomainEvent domainEvent in domainEvents.Where(
                x => typeof(IHaveExternalEvent).IsAssignableFrom(x.GetType())
            )
        )
        {
            Type genericType = typeof(IntegrationEventWrapper<>).MakeGenericType(domainEvent.GetType());

            IIntegrationEvent? domainNotificationEvent = (IIntegrationEvent?)
                Activator.CreateInstance(genericType, domainEvent);

            if (domainNotificationEvent is not null)
                yield return domainNotificationEvent;
        }
    }
}
