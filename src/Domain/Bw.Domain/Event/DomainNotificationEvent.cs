using Bw.Core.Domain.Event.Internal;
using Bw.Cqrs.Event;

namespace Bw.Domain.Event;

public abstract record DomainNotificationEvent : EventBase, IDomainNotificationEvent
{
}