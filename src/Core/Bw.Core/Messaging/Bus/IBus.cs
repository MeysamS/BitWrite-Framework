namespace Bw.Core.Messaging.Bus;

public interface IBus : IBusProducer, IBusConsumer
{
}


public class UnitBus : IBus
{
    public Task Consume(Type messageType, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task ConsumeAll(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task ConsumeAllFromAssemblyOf<TType>(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task ConsumeAllFromAssemblyOf(CancellationToken cancellationToken = default, params Type[] assemblyMarkerTypes)
    {
        return Task.CompletedTask;
    }

    public Task PublishAsync(object message, IDictionary<string, object?>? headers, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task PublishAsync(object message, IDictionary<string, object?>? headers, string? exchangeOrTopic = null, string? queue = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    void IBusConsumer.Consume<TMessage>(IMessageHandler<TMessage> handler, Action<IConsumeConfigurationBuilder>? consumeBuilder)
    {
        return;
    }

    Task IBusConsumer.Consume<TMessage>(MessageHandler<TMessage> subscribeMethod, Action<IConsumeConfigurationBuilder>? consumeBuilder, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    Task IBusConsumer.Consume<TMessage>(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    Task IBusConsumer.Consume<THandler, TMessage>(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    Task IBusProducer.PublishAsync<TMessage>(TMessage message, IDictionary<string, object?>? headers, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    Task IBusProducer.PublishAsync<TMessage>(TMessage message, IDictionary<string, object?>? headers, string? exchangeOrTopic, string? queue, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}