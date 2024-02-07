using Bw.Core.Domain.Event.Internal;

namespace Bw.EventSourcing.Projections;

public interface IHaveReadProjection
{
    Task ProjectAsync<T>(IStreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : IDomainEvent;
}