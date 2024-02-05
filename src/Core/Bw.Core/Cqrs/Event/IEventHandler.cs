using MediatR;

namespace Bw.Core.Cqrs.Event;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification
{ }