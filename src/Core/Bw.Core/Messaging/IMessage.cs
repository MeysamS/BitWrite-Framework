using MediatR;

namespace Bw.Core.Messaging;

public interface IMessage : INotification
{
    Guid MessageId { get; }
    DateTime Created { get; }
}