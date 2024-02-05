using Bw.Core.Cqrs.Event;

namespace Bw.Core.Domain.Event.Internal;

public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
}