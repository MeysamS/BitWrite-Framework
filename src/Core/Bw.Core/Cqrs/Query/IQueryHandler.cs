using MediatR;

namespace Bw.Core.Cqrs.Query;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}


public interface IStreamQueryHandler<in TQuery, TResponse> : IStreamRequestHandler<TQuery, TResponse>
    where TQuery : IStreamQuery<TResponse>
    where TResponse : notnull
{ }
