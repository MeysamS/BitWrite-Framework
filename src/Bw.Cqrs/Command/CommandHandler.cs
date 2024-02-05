using Bw.Core.Cqrs.Commands;
using MediatR;

namespace Bw.Cqrs.Command;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    protected abstract Task<Unit> HandleCommandAsync(TCommand command, CancellationToken cancellationToken);
    public Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
    {
        return HandleCommandAsync(request, cancellationToken);
    }
}