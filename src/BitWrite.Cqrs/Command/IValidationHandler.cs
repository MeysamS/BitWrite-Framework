namespace BitWrite.Cqrs.Command;

public interface IValidationHandler<in TCommand> : IDisposable
    where TCommand : ICommand
{
    IEnumerable<ValidationResult> Validate(TCommand command);
}