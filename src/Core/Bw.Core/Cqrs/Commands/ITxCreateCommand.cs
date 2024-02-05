using MediatR;

namespace Bw.Core.Cqrs.Commands;

public interface ITxCreateCommand<out TResponse> : ICommand<TResponse>, ITxRequest
    where TResponse : notnull
{ }


public interface ITxCreateCommand : ITxCreateCommand<Unit> { }
