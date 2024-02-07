using Bw.Core.Domain.Event.Internal;
using Bw.Core.Messaging;

namespace Bw.Domain.Event;

public class DomainNotificationEventPublisher : IDomainNotificationEventPublisher
{
    private readonly IMessagePersistenceService _messagePersistenceService;

    public DomainNotificationEventPublisher(IMessagePersistenceService messagePersistenceService)
    {
        _messagePersistenceService = messagePersistenceService;
    }


    public Task PublishAsync(IDomainNotificationEvent domainNotificationEvent, CancellationToken cancellationToken = default)
    {
        return _messagePersistenceService.AddNotificationAsync(domainNotificationEvent, cancellationToken);
    }

    public async Task PublishAsync(IDomainNotificationEvent[] domainNotificationEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainNotificationEvent in domainNotificationEvents)
        {
            await _messagePersistenceService.AddNotificationAsync(domainNotificationEvent, cancellationToken);
        }
    }
}
