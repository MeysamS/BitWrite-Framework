using Bw.Core.Domain.Event.Internal;
using Bw.Core.Domain.Model;
using Bw.Core.Domain.Model.Aggregate;
using Bw.Core.Domain.Model.Identity;
using Bw.Domain.Exceptions.Types;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Bw.Domain.Model;

public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
       where TId : notnull
{
    [NonSerialized]
    private readonly ConcurrentQueue<IDomainEvent> _uncommittedDomainEvents = new();

    private const long NewAggregateVersion = 0;
    public long OriginalVersion { get; protected set; } = NewAggregateVersion;


    /// <summary>
    /// Add the <paramref name="domainEvent"/> to the aggregate pending changes event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (!_uncommittedDomainEvents.Any(x => Equals(x.EventId, domainEvent.EventId)))
        {
            _uncommittedDomainEvents.Enqueue(domainEvent);
        }
    }
    public void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    public IReadOnlyList<IDomainEvent> GetUncommittedDomainEvents()
    {
        return _uncommittedDomainEvents.ToImmutableList();
    }
    public IReadOnlyList<IDomainEvent> DequeueUncommittedDomainEvents()
    {
        var events = _uncommittedDomainEvents.ToImmutableList();
        MarkUncommittedDomainEventAsCommitted();
        return events;
    }

    public bool HasUncommittedDomainEvents()
    {
        return _uncommittedDomainEvents.Any();
    }

    public void MarkUncommittedDomainEventAsCommitted()
    {
        _uncommittedDomainEvents.Clear();
    }

    public void ClearDomainEvents()
    {
        _uncommittedDomainEvents.Clear();
    }
}

public abstract class Aggregate<TIdentity, TId> : Aggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

public abstract class Aggregate : Aggregate<AggregateId, long>, IAggregate
{
}
