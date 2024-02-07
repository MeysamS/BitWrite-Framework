using Bw.Core.Domain.Event.Internal;
using Bw.Core.Domain.Model.Aggregate;
using Bw.Core.Domain.Model.Auditable;
using Bw.Persistence.EFCore.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Immutable;
using System.Data;
using System.Linq.Expressions;

namespace Bw.Persistence.EFCore;

public abstract class EfDbContextBase :
    DbContext,
    IDbFacadeResolver,
    IDbContext,
    IDomainEventContext
{
    private IDbContextTransaction? _currentTransaction;
    protected EfDbContextBase(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        AddingVersioning(modelBuilder);
        AddingSoftDeletes(modelBuilder);
    }
    private void AddingVersioning(ModelBuilder builder)
    {
        var types = builder.Model.GetEntityTypes()
            .Where(x => x.ClrType.IsAssignableFrom(typeof(IHaveAggregateVersion)));
        foreach (var entityType in types)
        {
            builder.Entity(entityType.ClrType).Property(nameof(IHaveAggregateVersion.OriginalVersion))
                .IsConcurrencyToken();
        }
    }

    private static void AddingSoftDeletes(ModelBuilder builder)
    {
        var types = builder.Model.GetEntityTypes()
            .Where(x => x.ClrType.IsAssignableTo(typeof(IHaveSoftDelete)));
        foreach (var entityType in types)
        {
            // 1. Add the IsDeleted Property
            entityType.AddProperty("IsDeleted", typeof(bool));

            // 2. Create the query filter
            var parameter = Expression.Parameter(entityType.ClrType);

            // EF.Property<bool>(TEntity, "IsDeleted")
            var propertyMethodInfo = typeof(EF)?.GetMethod("Property")?.MakeGenericMethod(typeof(bool));
            var isDeletedProperty = Expression.Call(propertyMethodInfo!, parameter, Expression.Constant("IsDeleted"));

            // EF.Property<bool>(TEntity, "IsDeleted") == false
            BinaryExpression compareExpression =
                Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));

            // TEntity => EF.Property<bool>(TEntity, "IsDeleted") == false
            var lambda = Expression.Lambda(compareExpression, parameter);

            builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database
                .BeginTransactionAsync(cancellationToken);
            try
            {
                await action();

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database
                .BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await action();

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public IReadOnlyList<IDomainEvent> GetAllUncommittedEvents()
    {
        var domainEvents = ChangeTracker.Entries<IHaveAggregate>()
            .Where(x => x.Entity.GetUncommittedDomainEvents().Any())
            .SelectMany(x => x.Entity.GetUncommittedDomainEvents())
            .ToList();
        return domainEvents.ToImmutableList();
    }

    public void MarkUncommittedDomainEventAsCommitted()
    {
        ChangeTracker.Entries<IHaveAggregate>()
            .Where(x => x.Entity.GetUncommittedDomainEvents().Any()).ToList()
            .ForEach(x => x.Entity.MarkUncommittedDomainEventAsCommitted());
    }

    public Task RetryOnExceptionAsync(Func<Task> operation)
    {
        return Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
    {
        return Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }


    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }


    // Ref: https://www.meziantou.net/entity-framework-core-generate-tracking-columns.htm
    // Ref: https://www.meziantou.net/entity-framework-core-soft-delete-using-query-filters.htm
    private void OnBeforeSaving()
    {
        var now = DateTime.Now;

        foreach (var entry in ChangeTracker.Entries<IHaveAggregate>())
        {
            // Ref: http://www.kamilgrzybek.com/design/handling-concurrency-aggregate-pattern-and-ef-core/
            var events = entry.Entity.GetUncommittedDomainEvents();
            if (events.Any())
            {
                entry.CurrentValues[nameof(IHaveAggregateVersion.OriginalVersion)] = entry.Entity.OriginalVersion + 1;
            }
        }

        // var userId = GetCurrentUser(); // TODO: Get current user
        foreach (var entry in ChangeTracker.Entries<IHaveAudit>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues[nameof(IHaveAudit.UpdatedDate)] = now;
                    entry.CurrentValues[nameof(IHaveAudit.UpdatorId)] = 1;
                    break;
                case EntityState.Added:
                    entry.CurrentValues[nameof(IHaveAudit.CreatedDate)] = now;
                    entry.CurrentValues[nameof(IHaveAudit.CreatorId)] = 1;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<IHaveCreator>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.CurrentValues[nameof(IHaveCreator.CreatedDate)] = now;
                entry.CurrentValues[nameof(IHaveCreator.CreatorId)] = 1;
            }
        }

        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is IHaveSoftDelete)
                        entry.CurrentValues["IsDeleted"] = false;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is IHaveSoftDelete)
                    {
                        entry.State = EntityState.Modified;
                        Entry(entry.Entity).CurrentValues["IsDeleted"] = true;
                    }

                    break;
            }
        }
    }

}
