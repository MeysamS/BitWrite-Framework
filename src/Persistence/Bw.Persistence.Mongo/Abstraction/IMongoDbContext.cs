﻿using MongoDB.Driver;

namespace Bw.Persistence.Mongo.Abstraction;

public interface IMongoDbContext : IDisposable
{
    IMongoCollection<T> GetCollection<T>(string? name = null);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransaction(CancellationToken cancellationToken = default);
    void AddCommand(Func<Task> func);
    IClientSessionHandle? GetSession();

}
