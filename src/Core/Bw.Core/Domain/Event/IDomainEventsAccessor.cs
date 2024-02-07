using Bw.Core.Domain.Event.Internal;

namespace Bw.Core.Domain.Event;

public interface IDomainEventsAccessor
{
    IReadOnlyList<IDomainEvent> UnCommittedDomainEvents { get; }
}