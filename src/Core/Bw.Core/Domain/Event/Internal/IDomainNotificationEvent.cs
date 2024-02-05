using Bw.Core.Cqrs.Event;

namespace Bw.Core.Domain.Event.Internal;

public interface IDomainNotificationEvent : IEvent
{
}

public interface IDomainNotificationEvent<TDomainEventType> : IDomainNotificationEvent
    where TDomainEventType : IDomainEvent
{
    TDomainEventType DomainEvent { get; set; }
}