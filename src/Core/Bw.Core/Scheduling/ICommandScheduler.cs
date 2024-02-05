using Bw.Core.Cqrs.Commands;

namespace Bw.Core.Scheduling;

public interface ICommandScheduler
{
    Task ScheduleAsync(
    IInternalCommand internalCommandCommand,
    CancellationToken cancellationToken = default);

    Task ScheduleAsync(
        IInternalCommand[] internalCommandCommands,
        CancellationToken cancellationToken = default);
}
