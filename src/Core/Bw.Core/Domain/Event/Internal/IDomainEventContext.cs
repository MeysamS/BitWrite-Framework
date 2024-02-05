namespace Bw.Core.Domain.Event.Internal;

public interface IDomainEventContext
{
    IReadOnlyList<IDomainEvent> GetAllUncommittedEvents();
    void MarkUncommittedDomainEventAsCommitted();
}