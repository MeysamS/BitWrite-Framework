using Bw.Core.Domain.Event.Internal;
using Bw.Core.Messaging;

namespace Bw.Messaging.Event;

public static partial class DomainEventsExtensions
{
    public static IEnumerable<IIntegrationEvent> GetWrappedIntegrationEvents(this IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents.Where(x => typeof(IHaveExternalEvent).IsAssignableFrom(x.GetType())))
        {
            Type genericType = typeof(IntegrationEventWrapper<>).MakeGenericType(domainEvent.GetType());
            IIntegrationEvent? domainNotificationEvent = (IIntegrationEvent?)Activator.CreateInstance(genericType, domainEvent);
            if (domainNotificationEvent is not null)
                yield return domainNotificationEvent;
        }
    }
}