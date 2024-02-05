using Bw.Core.Domain.Event;
using Bw.Core.Domain.Event.Internal;
using Bw.Persistence.EFCore.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Bw.Persistence.EFCore;

// https://github.com/Daniel127/EF-Unit-Of-Work
public class EfUnitOfWork<TDbContext> : IEfUnitOfWork<TDbContext>
    where TDbContext : EfDbContextBase
{
    private readonly TDbContext _dbContext;
    private readonly IDomainEventsAccessor _domainEventsAccessor;
    private readonly IDomainEventPublisher _domainEventPublisher;
    private readonly ILogger<EfUnitOfWork<TDbContext>> _logger;

    public EfUnitOfWork(TDbContext dbContext, IDomainEventsAccessor domainEventsAccessor, IDomainEventPublisher domainEventPublisher, ILogger<EfUnitOfWork<TDbContext>> logger)
    {
        _dbContext = dbContext;
        _domainEventsAccessor = domainEventsAccessor;
        _domainEventPublisher = domainEventPublisher;
        _logger = logger;
    }

    public TDbContext DbContext => _dbContext;

    public DbSet<TEntity> Set<TEntity>()
        where TEntity : class
    {
        return _dbContext.Set<TEntity>();
    }



    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = _domainEventsAccessor.UnCommittedDomainEvents;
        await _domainEventPublisher.PublishAsync(domainEvents.ToArray(), cancellationToken);

        await _dbContext.CommitTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = _domainEventsAccessor.UnCommittedDomainEvents;
        await _domainEventPublisher.PublishAsync(domainEvents.ToArray(), cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }


    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
        => _dbContext.ExecuteTransactionalAsync(action, cancellationToken);

    public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
        => _dbContext.ExecuteTransactionalAsync(action, cancellationToken);

    public Task RetryOnExceptionAsync(Func<Task> operation)
        => _dbContext.RetryOnExceptionAsync(operation);

    public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
        => _dbContext.RetryOnExceptionAsync(operation);

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        => _dbContext.RollbackTransactionAsync(cancellationToken);
}