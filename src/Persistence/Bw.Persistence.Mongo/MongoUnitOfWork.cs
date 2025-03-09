using Bw.Core.Persistence;
using Bw.Persistence.Mongo.Abstraction;

namespace Bw.Persistence.Mongo;

public class MongoUnitOfWork(IMongoDbContext mongoDbContext) : IMongoUnitOfWork, ITransactionAble    
{

    public IMongoDbContext Context => mongoDbContext;

  
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.BeginTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.RollbackTransaction(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.CommitTransactionAsync(cancellationToken);
    }

    public void Dispose() => Context.Dispose();
}

