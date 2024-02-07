using Bw.Core.Domain.Model.Entity;
using Bw.Core.Domain.Model.Identity;

namespace Bw.Domain.Model;

public abstract class Entity<TId> : IEntity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    public DateTime CreatedDate { get; private set; } = default!;

    public int? CreatorId { get; private set; } = default!;
}


public abstract class Entity<TIdentity, TId> : Entity<TIdentity>
    where TIdentity : Identity<TId>
{ }

public abstract class Entity : Entity<EntityId, long>, IEntity { }