using Bw.Core.Domain.Model.Auditable;
using Bw.Core.Domain.Model.Identity;

namespace Bw.Domain.Model;

public abstract class AuditAggregate<TId> : Aggregate<TId>, IAuditableEntity<TId>
    where TId : notnull
{

    public DateTime? UpdatedDate { get; protected set; } = default!;

    public int? UpdatorId { get; protected set; } = default!;

}


public abstract class AuditAggregate<TIdentity, TId> : AuditAggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

public abstract class AuditAggregate : AuditAggregate<Identity<long>, long>
{
}

