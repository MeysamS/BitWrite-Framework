using Bw.Core.Cqrs.Commands;
using Bw.Core.Domain.Event.Internal;

namespace Bw.Core.Domain.Event.Mapper;

public interface IInternalCommandMapper
{
    IReadOnlyList<IInternalCommand?>? MapToInternalCommands(IReadOnlyList<IDomainEvent> domainEvents);
    IInternalCommand? MapToInternalCommand(IDomainEvent domainEvent);
}
