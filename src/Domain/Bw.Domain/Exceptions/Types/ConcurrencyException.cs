namespace Bw.Domain.Exceptions.Types;

public class ConcurrencyException<TId> : DomainException
{
    public ConcurrencyException(TId id)
        : base($"A different version than expected was found in aggregate {id}")
    {
    }
}
