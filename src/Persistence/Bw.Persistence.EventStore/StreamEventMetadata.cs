using Bw.EventSourcing;

namespace Bw.Persistence.EventStore;

public record StreamEventMetadata(string EventId, long StreamPosition) : IStreamEventMetadata
{
    public long? LogPosition { get; }
}