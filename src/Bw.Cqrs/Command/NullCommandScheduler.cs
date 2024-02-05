using Bw.Core.Cqrs.Commands;
using Bw.Core.Scheduling;

namespace Bw.Cqrs.Command;

public class NullCommandScheduler : ICommandScheduler
{

    public Task ScheduleAsync(IInternalCommand internalCommandCommand, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task ScheduleAsync(IInternalCommand[] internalCommandCommands, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}