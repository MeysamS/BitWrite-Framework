using Bw.Core.Domain.Model.Identity;

namespace Bw.Persistence.EFCore.Abstraction;

public interface IPageRepository<TEntity, TKey>
    where TEntity : IHaveIdentity<TKey>
{ }

public interface IPageRepository<TEntity> : IPageRepository<TEntity, Guid>
    where TEntity : IHaveIdentity<Guid>
{
}
