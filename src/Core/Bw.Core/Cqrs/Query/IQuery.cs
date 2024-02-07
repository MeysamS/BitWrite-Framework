using MediatR;

namespace Bw.Core.Cqrs.Query;

public interface IQuery<out T> : IRequest<T>
    where T : notnull
{ }


public interface IStreamQuery<out T> : IStreamRequest<T>
    where T : notnull
{ }