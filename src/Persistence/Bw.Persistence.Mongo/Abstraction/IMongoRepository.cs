using Bw.Core.Domain.Model.Identity;
using Bw.Core.Persistence;

namespace Bw.Persistence.Mongo.Abstraction;

public interface IMongoRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IHaveIdentity<TId>
{
}
