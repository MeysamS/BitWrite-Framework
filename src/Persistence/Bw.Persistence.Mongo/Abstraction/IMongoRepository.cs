using Bw.Core.Domain.Model.Identity;
using Bw.Core.Persistence;
using MongoDB.Driver.Linq;

namespace Bw.Persistence.Mongo.Abstraction;

public interface IMongoRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IHaveIdentity<TId>
{
}
