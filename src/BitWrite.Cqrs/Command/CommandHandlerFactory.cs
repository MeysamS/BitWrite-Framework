using BitWrite.Cqrs.Command.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace BitWrite.Cqrs.Command;

public class CommandHandlerFactory : ICommandHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CommandHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICommandHandler<TCommand> Create<TCommand>() where TCommand : ICommand
    {
        var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
        if (handler is null)
        {
            throw new CommandHandlerNotFoundException(typeof(TCommand));
        }

        return handler;
    }

    public ICommandHandler<TCommand, TResult> Create<TCommand, TResult>()
        where TCommand : ICommand
        where TResult : IResult
    {
        var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>();
        if (handler is null)
        {
            throw new CommandHandlerNotFoundException(typeof(TCommand));
        }
        return handler;
    }
}