using System.ComponentModel.Design;

namespace BitWrite.Cqrs.Command;

public interface ICommandHandlerFactory
{
    ICommandHandler<TCommand> Create<TCommand>() where TCommand : ICommand;

    ICommandHandler<TCommand, TResult> Create<TCommand, TResult>()
        where TCommand : ICommand
        where TResult : IResult;

}