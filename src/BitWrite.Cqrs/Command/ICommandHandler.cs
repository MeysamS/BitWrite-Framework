namespace BitWrite.Cqrs.Command;

public interface ICommandHandler<in TCommand, out TResult>
    where TCommand : ICommand
    where TResult : IResult
{
    Task<IResult> HandleAsync(TCommand command);
}

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command);
}