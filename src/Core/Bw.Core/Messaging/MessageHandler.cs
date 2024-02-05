using Bw.Core.Messaging.Context;
using Bw.Core.Messaging.Persistence;

namespace Bw.Core.Messaging;

public delegate Task MessageHandler<in TMessage>(
    IConsumeContext<TMessage> context,
    CancellationToken cancellationToken = default
) where TMessage : class, IMessage;

public delegate Task<Acknowledgement> MessageHandlerAck<in TMessage>(
    IConsumeContext<TMessage> context,
    CancellationToken cancellationToken = default
)
    where TMessage : class, IMessage;
