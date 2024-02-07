namespace Bw.EventSourcing;

public interface IStreamEventMetadata
{
    string EventId { get; }
    long? LogPosition { get; }
    long StreamPosition { get; }
}
