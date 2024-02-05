using Bw.Core.Cqrs.Event;
using Bw.Core.Domain.Event;
using Bw.Core.Domain.Event.Internal;
using Bw.Core.Domain.Event.Mapper;
using Bw.Core.Domain.Messaging;
using Bw.Core.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace Bw.DomainEvent;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IEventProcessor _eventProcessor;
    private readonly IMessagePersistenceService _messagePersistenceService;
    private readonly IDomainEventsAccessor _domainEventsAccessor;
    private readonly IDomainNotificationEventPublisher _domainNotificationEventPublisher;
    private readonly IServiceProvider _serviceProvider;

    public DomainEventPublisher(
        IEventProcessor eventProcessor,
        IMessagePersistenceService messagePersistenceService,
        IDomainEventsAccessor domainEventsAccessor,
        IDomainNotificationEventPublisher domainNotificationEventPublisher,
        IServiceProvider serviceProvider)
    {
        _eventProcessor = eventProcessor;
        _messagePersistenceService = messagePersistenceService;
        _domainEventsAccessor = domainEventsAccessor;
        _domainNotificationEventPublisher = domainNotificationEventPublisher;
        _serviceProvider = serviceProvider;
    }

    public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return PublishAsync(new[] { domainEvent }, cancellationToken);
    }

    public async Task PublishAsync(IDomainEvent[] domainEvents, CancellationToken cancellationToken = default)
    {
        if (!domainEvents.Any())
            return;

        // https://github.com/dotnet-architecture/eShopOnContainers/issues/700#issuecomment-461807560
        // https://github.com/dotnet-architecture/eShopOnContainers/blob/e05a87658128106fef4e628ccb830bc89325d9da/src/Services/Ordering/Ordering.Infrastructure/OrderingContext.cs#L65
        // http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
        // http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
        // https://www.ledjonbehluli.com/posts/domain_to_integration_event/

        // Dispatch our domain events before commit
        var eventsToDispatch = domainEvents.ToList();
        if (!eventsToDispatch.Any())
        {
            eventsToDispatch = new List<IDomainEvent>(_domainEventsAccessor.UnCommittedDomainEvents);
        }
        await _eventProcessor.DispatchAsync(eventsToDispatch.ToArray(), cancellationToken);

        // Save wrapped integration and notification events to outbox for further processing after commit
        var wrappedNotificationEvents = eventsToDispatch.GetWrappedDomainNotificationEvents().ToArray();
        await _domainNotificationEventPublisher.PublishAsync(wrappedNotificationEvents, cancellationToken);


        var wrappedIntegrationEvents = eventsToDispatch.GetWrappedIntegrationEvents().ToArray();
        foreach (var wrappedIntegrationEvent in wrappedIntegrationEvents)
        {
            await _messagePersistenceService.AddPublishMessageAsync(
                new MessageEnvelope(wrappedIntegrationEvent, new Dictionary<string, object?>()),
                cancellationToken
            );
        }

        var eventMappers = _serviceProvider.GetServices<IEventMapper>().ToImmutableList();

        // Save event mapper events into outbox for further processing after commit
        var integrationEvents = GetIntegrationEvents(_serviceProvider, eventMappers, eventsToDispatch);
        if (integrationEvents.Any())
        {
            foreach (var integrationEvent in integrationEvents)
            {
                await _messagePersistenceService.AddPublishMessageAsync(
                    new MessageEnvelope(integrationEvent, new Dictionary<string, object?>()),
                    cancellationToken
                );
            }
        }

        var notificationEvents = GetNotificationEvents(_serviceProvider, eventMappers, eventsToDispatch);

        if (notificationEvents.Any())
        {
            foreach (var notification in notificationEvents)
            {
                await _messagePersistenceService.AddNotificationAsync(notification, cancellationToken);
            }
        }

    }


    private IReadOnlyList<IDomainNotificationEvent> GetNotificationEvents(
        IServiceProvider serviceProvider,
        IReadOnlyList<IEventMapper> eventMappers,
        IReadOnlyList<IDomainEvent> eventsToDispatch
        )
    {
        var notificationEventmappers = serviceProvider.GetServices<IIDomainNotificationEventMapper>().ToImmutableList();

        List<IDomainNotificationEvent> notificationEvents = new List<IDomainNotificationEvent>();

        if (eventMappers.Any())
        {
            foreach (var eventMapper in eventMappers)
            {
                var items = eventMapper.MapToDomainNotificationEvents(eventsToDispatch)?.ToList();
                if (items is not null && items.Any())
                {
                    notificationEvents.AddRange(items.Where(x => x is not null)!);
                }
            }
        }
        else if (notificationEventmappers.Any())
        {
            foreach (var notificationEventMapper in notificationEventmappers)
            {
                var items = notificationEventMapper.MapToDomainNotificationEvents(eventsToDispatch)?.ToList();
                if (items is not null && items.Any())
                {
                    notificationEvents.AddRange(items.Where(x => x is not null)!);
                }
            }
        }
        return notificationEvents.ToImmutableList();
    }

    private static IReadOnlyList<IIntegrationEvent> GetIntegrationEvents(
       IServiceProvider serviceProvider,
       IReadOnlyList<IEventMapper> eventMappers,
       IReadOnlyList<IDomainEvent> eventsToDispatch
   )
    {
        var integrationEventMappers = serviceProvider.GetServices<IIntegrationEventMapper>().ToImmutableList();

        List<IIntegrationEvent> integrationEvents = new List<IIntegrationEvent>();

        if (eventMappers.Any())
        {
            foreach (var eventMapper in eventMappers)
            {
                var items = eventMapper.MapToIntegrationEvents(eventsToDispatch)?.ToList();
                if (items is not null && items.Any())
                {
                    integrationEvents.AddRange(items.Where(x => x is not null)!);
                }
            }
        }
        else if (integrationEventMappers.Any())
        {
            foreach (var integrationEventMapper in integrationEventMappers)
            {
                var items = integrationEventMapper.MapToIntegrationEvents(eventsToDispatch)?.ToList();
                if (items is not null && items.Any())
                {
                    integrationEvents.AddRange(items.Where(x => x is not null)!);
                }
            }
        }

        return integrationEvents.ToImmutableList();
    }
}
