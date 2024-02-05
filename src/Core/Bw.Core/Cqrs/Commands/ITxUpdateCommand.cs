using MediatR;

namespace Bw.Core.Cqrs.Commands;

public interface ITxUpdateCommand<out TResponse> : IUpdateCommand<TResponse>, ITxRequest
    where TResponse : notnull
{ }


public interface ITxUpdateCommand : ITxUpdateCommand<Unit> { }