using Bw.Core.Cqrs.Query;

namespace Bw.Cqrs.Query;

public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    protected abstract Task<TResponse> HandleQueryAsync(TQuery query, CancellationToken cancellationToken = default);

    public Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken) =>
        HandleQueryAsync(request, cancellationToken);

}