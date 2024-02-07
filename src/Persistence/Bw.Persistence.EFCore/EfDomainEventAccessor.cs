using Bw.Core.Domain.Event;
using Bw.Core.Domain.Event.Internal;

namespace Bw.Persistence.EFCore;

public class EfDomainEventAccessor : IDomainEventsAccessor
{
    private readonly IDomainEventContext _domainEventContext;
    private readonly IAggregatesDomainEventsRequestStore _aggregatesDomainEventsRequestStore;

    public EfDomainEventAccessor(IDomainEventContext domainEventContext, IAggregatesDomainEventsRequestStore aggregatesDomainEventsRequestStore)
    {
        _domainEventContext = domainEventContext;
        _aggregatesDomainEventsRequestStore = aggregatesDomainEventsRequestStore;
    }

    public IReadOnlyList<IDomainEvent> UnCommittedDomainEvents
    {
        get
        {
            _ = _aggregatesDomainEventsRequestStore.GetAllUncommittedEvents();
            // Or
            return _domainEventContext.GetAllUncommittedEvents();
        }
    }
}