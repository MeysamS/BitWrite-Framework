using Bw.Core.Domain.Event.Internal;

namespace Bw.EventSourcing.Projections;

public interface IReadProjectionPublisher
{
    Task PublishAsync(IStreamEvent streamEvent, CancellationToken cancellationToken = default);

    Task PublishAsync<T>(IStreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : IDomainEvent;
}