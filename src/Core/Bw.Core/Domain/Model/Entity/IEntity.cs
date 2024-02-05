using Bw.Core.Domain.Model.Auditable;
using Bw.Core.Domain.Model.Identity;

namespace Bw.Core.Domain.Model.Entity;

public interface IEntity<out TId> : IHaveIdentity<TId>, IHaveCreator { }


public interface IEntity<out TIdentity, in TId> : IEntity<TIdentity>
    where TIdentity : IIdentity<TId>
{ }

public interface IEntity : IEntity<EntityId> { }
