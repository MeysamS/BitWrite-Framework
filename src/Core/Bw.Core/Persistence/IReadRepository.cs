using Bw.Core.Domain.Model.Identity;
using System.Linq.Expressions;

namespace Bw.Core.Persistence;

public interface IReadRepository<TEntity, in TId>
    where TEntity : class, IHaveIdentity<TId>
{
    Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default);

    Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
