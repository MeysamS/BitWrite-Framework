namespace Bw.Core.Cqrs.Query;

public interface IListQuery<out TResponse> : IPageRequest, IQuery<TResponse>
where TResponse : notnull
{ }