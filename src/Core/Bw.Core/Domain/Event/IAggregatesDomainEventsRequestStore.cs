using Bw.Core.Domain.Event.Internal;
using Bw.Core.Domain.Model.Aggregate;

namespace Bw.Core.Domain.Event;

public interface IAggregatesDomainEventsRequestStore
{
    IReadOnlyList<IDomainEvent> AddEventsFromAggregate<T>(T aggregate)
      where T : IHaveAggregate;

    void AddEvents(IReadOnlyList<IDomainEvent> events);

    IReadOnlyList<IDomainEvent> GetAllUncommittedEvents();
}