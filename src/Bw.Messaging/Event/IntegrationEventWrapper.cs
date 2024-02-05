
using Bw.Core.Domain.Event.Internal;

namespace Bw.Messaging.Event
{
    public record IntegrationEventWrapper<TDomainEventType> : IntegrationEvent
        where TDomainEventType : IDomainEvent;

}
