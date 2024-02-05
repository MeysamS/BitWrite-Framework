﻿using Bw.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Bw.Persistence.EFCore.Abstraction;

public interface IDbContext : ITxDbContextExecution, IRetryDbContextExecution
{
    DbSet<TEntity> Set<TEntity>()
    where TEntity : class;

    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}