namespace BitWrite.Cqrs.Command.Exceptions;

public class CommandHandlerNotFoundException : Exception
{
    public CommandHandlerNotFoundException(Type type)
        : base($"Command handler not found for command type: {type}")
    {
    }
}