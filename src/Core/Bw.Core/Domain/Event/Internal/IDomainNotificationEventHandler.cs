using Bw.Core.Cqrs.Event;

namespace Bw.Core.Domain.Event.Internal;

public interface IDomainNotificationEventHandler<in TEvent> : IEventHandler<TEvent>
where TEvent : IDomainNotificationEvent
{ }