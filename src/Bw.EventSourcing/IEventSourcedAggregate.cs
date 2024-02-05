using Bw.Core.Domain.Model.Aggregate;
using Bw.Core.Domain.Model.Entity;
using Bw.Core.Domain.Model.Identity;

namespace Bw.EventSourcing;

public interface IEventSourcedAggregate<out TId> : IEntity<TId>, IHaveEventSourcingAggregate
where TId : notnull
{ }

public interface IEventSourcedAggregate<out TIdentity, TId> : IEventSourcedAggregate<TIdentity>
    where TIdentity : Identity<TId>
{ }

public interface IEventSourcedAggregate : IEventSourcedAggregate<AggregateId, long> { }

