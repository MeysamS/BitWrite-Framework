using Bw.Core.Cqrs.Event;
using Bw.Core.Utils;

namespace Bw.Cqrs.Event
{
    public abstract record EventBase : IEvent
    {
        public Guid EventId { get; protected set; } = Guid.NewGuid();

        public long EventVersion { get; protected set; } = -1;

        public DateTime OccurredOn { get; protected set; } = DateTime.Now;

        public DateTimeOffset TimeStamp { get; protected set; } = DateTimeOffset.Now;

        public string EventType => TypeMapper.GetFullTypeName(GetType());
    }
}