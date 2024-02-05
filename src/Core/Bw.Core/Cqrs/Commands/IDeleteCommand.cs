namespace Bw.Core.Cqrs.Commands;
public interface IDeleteCommand<TId, out TResponse> : ICommand<TResponse>
    where TId : struct
    where TResponse : notnull
{ }


public interface IDeleteCommand<TId> : ICommand
    where TId : struct
{
    public TId Id { get; init; }
}