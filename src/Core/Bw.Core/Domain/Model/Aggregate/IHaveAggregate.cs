using Bw.Core.Domain.Event.Internal;

namespace Bw.Core.Domain.Model.Aggregate;

public interface IHaveAggregate : IHaveDomainEvents, IHaveAggregateVersion
{
}
