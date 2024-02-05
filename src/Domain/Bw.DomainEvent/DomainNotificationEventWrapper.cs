
using Bw.Core.Domain.Event.Internal;

namespace Bw.DomainEvent;

public record DomainNotificationEventWrapper<TDomainEventType>(TDomainEventType DomainEvent) : DomainNotificationEvent
    where TDomainEventType : IDomainEvent;
