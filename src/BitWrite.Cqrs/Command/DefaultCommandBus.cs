namespace BitWrite.Cqrs.Command;

public class DefaultCommandBus : ICommandBus
{
    private readonly ICommandHandlerFactory _commandHandlerFactory;

    public DefaultCommandBus(ICommandHandlerFactory commandHandlerFactory)
    {
        _commandHandlerFactory = commandHandlerFactory;
    }

    public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = _commandHandlerFactory.Create<TCommand>();
        await handler.HandleAsync(command);
    }

    public async Task<TResult?> DispatchAsync<TCommand, TResult>(TCommand command)
        where TCommand : ICommand
        where TResult : class, IResult
    {
        var handler = _commandHandlerFactory.Create<TCommand, TResult>();
        return await handler.HandleAsync(command).ConfigureAwait(false) as TResult;
    }


    public void Dispatch<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = _commandHandlerFactory.Create<TCommand>();
        handler.HandleAsync(command);
    }


}