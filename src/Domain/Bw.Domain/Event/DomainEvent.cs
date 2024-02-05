
using Bw.Core.Domain.Event.Internal;
using Bw.Cqrs.Event;
namespace Bw.Domain.Event;

public abstract record DomainEvent : EventBase, IDomainEvent
{
    public dynamic AggregateId { get; protected set; } = null!;

    public long AggregateSequenceNumber { get; protected set; }

    public virtual IDomainEvent WithAggregate(dynamic aggregateId, long version)
    {
        AggregateId = aggregateId;
        AggregateSequenceNumber = version;

        return this;
    }
}