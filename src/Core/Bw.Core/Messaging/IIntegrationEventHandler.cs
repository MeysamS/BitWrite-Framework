using Bw.Core.Cqrs.Event;

namespace Bw.Core.Messaging;

public interface IIntegrationEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{ }