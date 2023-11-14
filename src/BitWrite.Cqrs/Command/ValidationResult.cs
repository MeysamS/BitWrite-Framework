namespace BitWrite.Cqrs.Command;

public record ValidationResult(string? MemberName, string Message)
{
}