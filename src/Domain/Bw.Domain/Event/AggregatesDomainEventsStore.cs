﻿using Bw.Core.Domain.Event;
using Bw.Core.Domain.Event.Internal;
using Bw.Core.Domain.Model.Aggregate;

namespace Bw.Domain.Event;


public class AggregatesDomainEventsStore : IAggregatesDomainEventsRequestStore
{
    private readonly List<IDomainEvent> _uncommittedDomainEvents = new();

    public IReadOnlyList<IDomainEvent> AddEventsFromAggregate<T>(T aggregate)
        where T : IHaveAggregate
    {
        var events = aggregate.GetUncommittedDomainEvents();

        AddEvents(events);

        return events;
    }

    public void AddEvents(IReadOnlyList<IDomainEvent> events)
    {
        if (events.Any())
        {
            _uncommittedDomainEvents.AddRange(events);
        }
    }

    public IReadOnlyList<IDomainEvent> GetAllUncommittedEvents()
    {
        return _uncommittedDomainEvents;
    }
}

