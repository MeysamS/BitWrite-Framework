using MediatR;

namespace Bw.Core.Cqrs.Commands;

public interface ITxCommand : ITxCommand<Unit> { }

public interface ITxCommand<out T> : ICommand<T>, ITxRequest
    where T : notnull
{ }
