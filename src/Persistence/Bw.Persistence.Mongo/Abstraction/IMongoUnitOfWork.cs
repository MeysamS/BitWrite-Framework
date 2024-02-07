using Bw.Core.Persistence;

namespace Bw.Persistence.Mongo.Abstraction;

public interface IMongoUnitOfWork<out TContext> : IUnitOfWork<TContext>
    where TContext : class, IMongoDbContext
{ }