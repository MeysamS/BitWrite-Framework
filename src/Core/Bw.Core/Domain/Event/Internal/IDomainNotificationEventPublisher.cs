namespace Bw.Core.Domain.Event.Internal;

public interface IDomainNotificationEventPublisher
{
    Task PublishAsync(IDomainNotificationEvent domainNotificationEvent, CancellationToken cancellationToken = default);

    Task PublishAsync(
        IDomainNotificationEvent[] domainNotificationEvents,
        CancellationToken cancellationToken = default
    );
}