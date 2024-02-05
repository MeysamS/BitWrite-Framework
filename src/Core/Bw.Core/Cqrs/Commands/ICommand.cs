using MediatR;

namespace Bw.Core.Cqrs.Commands;

public interface ICommand<out T> : IRequest<T>
    where T : notnull
{
}

public interface ICommand : ICommand<Unit> { }