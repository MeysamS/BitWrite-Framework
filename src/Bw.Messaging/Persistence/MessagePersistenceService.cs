using Bw.Core.Cqrs.Commands;
using Bw.Core.Domain.Event.Internal;
using Bw.Core.Messaging;
using Bw.Core.Messaging.Bus;
using Bw.Core.Messaging.Persistence;
using Bw.Core.Utils;
using Bw.Messaging.Serializations;
using Bw.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Bw.Messaging.Persistence;

public class MessagePersistenceService : IMessagePersistenceService
{
    private readonly ILogger<MessagePersistenceService> _logger;
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageSerializer _messageSerializer;
    private readonly IMediator _mediator;
    private readonly IBus _bus;
    private readonly ISerializer _serializer;

    public MessagePersistenceService(
        ILogger<MessagePersistenceService> logger,
        IMessagePersistenceRepository messagePersistenceRepository,
        IMessageSerializer messageSerializer,
        IMediator mediator,
        IBus bus,
        ISerializer serializer)
    {
        _logger = logger;
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageSerializer = messageSerializer;
        _mediator = mediator;
        _bus = bus;
        _serializer = serializer;
    }

    public async Task AddNotificationAsync(
        IDomainNotificationEvent notification,
        CancellationToken cancellationToken = default)
    {
        await _messagePersistenceRepository.AddAsync(
            new StoreMessage(
                notification.EventId,
                TypeMapper.GetFullTypeName(notification.GetType()),
                _serializer.Serialize(notification),
                MessageDeliveryType.Internal),
            cancellationToken);
    }

    public async Task AddPublishMessageAsync<TMessageEnvelope>(
        TMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken = default) where TMessageEnvelope : MessageEnvelope
    {
        await AddMessageCore(messageEnvelope, MessageDeliveryType.Outbox, cancellationToken);
    }

    public async Task AddReceivedMessageAsync<TMessageEnvelope>(TMessageEnvelope messageEnvelope, CancellationToken cancellationToken = default) where TMessageEnvelope : MessageEnvelope
    {
        await AddMessageCore(messageEnvelope, MessageDeliveryType.Inbox, cancellationToken);
    }

    public Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
        Expression<Func<StoreMessage, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return _messagePersistenceRepository.GetByFilterAsync(predicate ?? (_ => true), cancellationToken);
    }

    public async Task ProcessAllAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _messagePersistenceRepository.GetByFilterAsync(
          x => x.MessageStatus != MessageStatus.Processed,
          cancellationToken
      );

        foreach (var message in messages)
        {
            await ProcessAsync(message.Id, cancellationToken);
        }
    }

    public async Task ProcessAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _messagePersistenceRepository.GetByIdAsync(messageId, cancellationToken);
        if (message is null)
            return;
        switch (message.DeliveryType)
        {
            case MessageDeliveryType.Inbox:
                await ProcessInbox(message, cancellationToken);
                break;
            case MessageDeliveryType.Outbox:
                await ProcessOutbox(message, cancellationToken);
                break;
            case MessageDeliveryType.Internal:
                await ProcessInternal(message, cancellationToken);
                break;
        }

        await _messagePersistenceRepository.ChangeStateAsync(message.Id, MessageStatus.Processed, cancellationToken);
    }

    async Task IMessagePersistenceService.AddInternalMessageAsync<TCommand>(TCommand internalCommand, CancellationToken cancellationToken)
    {
        await AddMessageCore(new MessageEnvelope(internalCommand), MessageDeliveryType.Internal, cancellationToken);
    }


    private async Task AddMessageCore(
        MessageEnvelope messageEnvelope,
        MessageDeliveryType deliveryType,
        CancellationToken cancellationToken = default)
    {
        Guid id;
        if (messageEnvelope.Message is IMessage im)
        {
            id = im.MessageId;
        }
        else if (messageEnvelope.Message is IInternalCommand command)
        {
            id = command.InternalCommandId;
        }
        else
        {
            id = Guid.NewGuid();
        }

        await _messagePersistenceRepository.AddAsync(
            new StoreMessage(
                id,
                TypeMapper.GetFullTypeName(messageEnvelope.Message!.GetType()),
                _messageSerializer.Serialize(messageEnvelope),
                deliveryType), cancellationToken
                );

        _logger.LogInformation(
            "Message with id: {MessageID} and delivery type: {DeliveryType} saved in persistence message store",
            id,
            deliveryType.ToString()
        );

    }

    private Task ProcessInbox(StoreMessage message, CancellationToken cancellationToken)
    {
        // TODO Contribution mehdi , meysam , hadi
        return Task.CompletedTask;

    }

    private async Task ProcessOutbox(StoreMessage message, CancellationToken cancellationToken)
    {
        MessageEnvelope? messageEnvelope = _messageSerializer.Deserialize<MessageEnvelope>(message.Data, true);
        if (messageEnvelope is null || messageEnvelope.Message is null)
            return;

        var data = _messageSerializer.Deserialize(
            messageEnvelope.Message.ToString()!,
            TypeMapper.GetType(message.DataType));

        if (data is IMessage)
        {
            // we should pass a object type message or explicit our message type, not cast to IMessage (data is IMessage integrationEvent) because masstransit doesn't work with IMessage cast.
            await _bus.PublishAsync(data, messageEnvelope.Headers, cancellationToken);
            _logger.LogInformation(
                "Message with id: {MessageId} and delivery type: {DeliveryType} processed from the persistence message store",
                message.Id,
                message.DeliveryType
            );
        }
    }


    private async Task ProcessInternal(StoreMessage message, CancellationToken cancellationToken)
    {
        MessageEnvelope? messageEnvelope = _messageSerializer.Deserialize<MessageEnvelope>(message.Data, true);
        if (messageEnvelope is null || messageEnvelope.Message is null)
            return;

        var data = _messageSerializer.Deserialize(
            messageEnvelope.Message.ToString()!,
            TypeMapper.GetType(message.DataType)
        );

        if (data is IDomainNotificationEvent domainNotificationEvent)
        {
            await _mediator.Publish(domainNotificationEvent, cancellationToken);

            _logger.LogInformation(
                "Domain-Notification with id: {EventID} and delivery type: {DeliveryType} processed from the persistence message store",
                message.Id,
                message.DeliveryType
            );
        }

        if (data is IInternalCommand internalCommand)
        {
            await _mediator.Send(internalCommand, cancellationToken);

            _logger.LogInformation(
                "InternalCommand with id: {EventID} and delivery type: {DeliveryType} processed from the persistence message store",
                message.Id,
                message.DeliveryType
            );
        }
    }



}
