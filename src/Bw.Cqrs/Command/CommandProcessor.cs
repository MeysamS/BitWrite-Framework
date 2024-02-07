using Bw.Core.Cqrs.Commands;
using Bw.Core.Scheduling;
using MediatR;


namespace Bw.Cqrs.Command;

public class CommandProcessor : ICommandProcessor
{
    private readonly IMediator _mediator;
    private readonly ICommandScheduler _commandScheduler;

    public CommandProcessor(IMediator mediator, ICommandScheduler commandScheduler)
    {
        _mediator = mediator;
        _commandScheduler = commandScheduler;
    }
    public async Task ScheduleAsync(IInternalCommand internalCommandCommand, CancellationToken cancellationToken = default)
    {
        await _commandScheduler.ScheduleAsync(internalCommandCommand, cancellationToken);
    }

    public async Task ScheduleAsync(IInternalCommand[] internalCommandCommands, CancellationToken cancellationToken = default)
    {
        foreach (var internalCommandCommand in internalCommandCommands)
        {
            await ScheduleAsync(internalCommandCommand, cancellationToken);
        }
    }

    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default) where TResult : notnull
    {
        return _mediator.Send(command, cancellationToken);
    }
}