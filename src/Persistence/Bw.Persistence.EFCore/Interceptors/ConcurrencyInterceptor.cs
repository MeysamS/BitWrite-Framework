﻿using Bw.Core.Domain.Event.Internal;
using Bw.Core.Domain.Model.Aggregate;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bw.Persistence.EFCore.Interceptors;


// https://khalidabuhakmeh.com/entity-framework-core-5-interceptors
// https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors#savechanges-interception
public class ConcurrencyInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in eventData.Context.ChangeTracker.Entries<IHaveDomainEvents>())
        {
            // Ref: http://www.kamilgrzybek.com/design/handling-concurrency-aggregate-pattern-and-ef-core/
            var events = entry.Entity.GetUncommittedDomainEvents();
            if (events.Any())
            {
                if (entry.Entity is IHaveAggregateVersion av)
                {
                    entry.CurrentValues[nameof(IHaveAggregateVersion.OriginalVersion)] = av.OriginalVersion + 1;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

