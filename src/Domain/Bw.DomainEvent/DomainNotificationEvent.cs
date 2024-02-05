

using Bw.Core.Domain.Event.Internal;
using Bw.MediatR.Event;

namespace Bw.DomainEvent;

public abstract record DomainNotificationEvent : Event, IDomainNotificationEvent
{
}