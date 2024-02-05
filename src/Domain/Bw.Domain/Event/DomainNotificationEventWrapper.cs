using Bw.Core.Domain.Event.Internal;

namespace Bw.Domain.Event;

public record DomainNotificationEventWrapper<TDomainEventType>(TDomainEventType DomainEvent) : DomainNotificationEvent
    where TDomainEventType : IDomainEvent;
