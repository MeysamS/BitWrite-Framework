using Bw.Core.Cqrs.Commands;
using Bw.Core.Messaging;
using Bw.Core.Scheduling;

namespace Bw.Cqrs.Command;

public class PersistanceCommandScheduler(IMessagePersistenceService messagePersistenceService) : ICommandScheduler
{

    public async Task ScheduleAsync(IInternalCommand internalCommandCommand, CancellationToken cancellationToken = default)
    {
        if (internalCommandCommand is null)
        {
            throw new ArgumentNullException(nameof(internalCommandCommand));
        }
        await messagePersistenceService.AddInternalMessageAsync(internalCommandCommand, cancellationToken);
    }

    public async Task ScheduleAsync(IInternalCommand[] internalCommandCommands, CancellationToken cancellationToken = default)
    {
        if (internalCommandCommands is null || internalCommandCommands.Length == 0)
        {
            throw new ArgumentException("Command list cannot be null or empty.", nameof(internalCommandCommands));
        }

        foreach (var command in internalCommandCommands)
        {
            await messagePersistenceService.AddInternalMessageAsync(command, cancellationToken);
        }

    }
}