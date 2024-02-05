using Bw.Core.Messaging;

namespace Bw.Messaging;

public record Message : IMessage
{
    public Guid MessageId => Guid.NewGuid();

    public DateTime Created { get; } = DateTime.Now;
}
