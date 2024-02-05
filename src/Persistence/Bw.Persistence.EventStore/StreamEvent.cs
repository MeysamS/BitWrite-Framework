using Bw.Core.Domain.Event.Internal;
using Bw.Cqrs.Event;
using Bw.EventSourcing;

namespace Bw.Persistence.EventStore;


public record StreamEvent<T>(T Data, IStreamEventMetadata? Metadata = null) : StreamEvent(Data, Metadata),
    IStreamEvent<T>
    where T : IDomainEvent
{
    public new T Data => (T)base.Data;
}

public record StreamEvent(IDomainEvent Data, IStreamEventMetadata? Metadata = null) : EventBase, IStreamEvent;

