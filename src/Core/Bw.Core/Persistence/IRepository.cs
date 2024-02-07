using Bw.Core.Domain.Model.Identity;

namespace Bw.Core.Persistence;

public interface IRepository<TEntity, in TId> :
    IReadRepository<TEntity, TId>,
    IWriteRepository<TEntity, TId>,
    IDisposable
    where TEntity : class, IHaveIdentity<TId>
{
}

public interface IRepository<TEntity> : IRepository<TEntity, long>
    where TEntity : class, IHaveIdentity<long>
{
}

