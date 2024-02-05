using Bw.Core.Domain.Event.Internal;
using Bw.Core.Domain.Model.Aggregate;
using Bw.EventSourcing.Projections;

namespace Bw.EventSourcing;

public interface IHaveEventSourcingAggregate :
    IHaveAggregateStateProjection,
    IHaveAggregate,
    IHaveEventSourcedAggregateVersion
{
    /// <summary>
    /// Loads the current state of the aggregate from a list of events.
    /// </summary>
    /// <param name="history">Domain events from the aggregate stream.</param>
    void LoadFromHistory(IEnumerable<IDomainEvent> history);
}
