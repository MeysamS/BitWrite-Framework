using Bw.Core.Domain.Event.Internal;
using Bw.Core.Reflection;
using Bw.EventSourcing;

namespace Bw.Persistence.EventStore.Extensions;

public static class StreamEventExtensions
{
    public static IStreamEvent ToStreamEvent(this IDomainEvent domainEvent, IStreamEventMetadata? metadata)
    {
        return ReflectionUtilities.CreateGenericType(
            typeof(StreamEvent),
            new[] { domainEvent.GetType() },
            domainEvent,
            metadata
        );
    }
}