using Bw.Core.Cqrs.Event;
using Bw.Core.Domain.Event.Internal;

namespace Bw.EventSourcing;

public interface IStreamEvent : IEvent
{
    public IDomainEvent Data { get; }

    public IStreamEventMetadata? Metadata { get; }
}

public interface IStreamEvent<out T> : IStreamEvent
    where T : IDomainEvent
{
    public new T Data { get; }
}
