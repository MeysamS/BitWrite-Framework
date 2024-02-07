using Bw.Core.Domain.Model;

namespace Bw.Domain.Model;


public abstract class ValueObject : IEquatable<ValueObject>, IValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        return Equals(other! as object);
    }


    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        using IEnumerator<object> thisValues = GetEqualityComponents().GetEnumerator();
        using IEnumerator<object> otherValues = other.GetEqualityComponents().GetEnumerator();
        while (thisValues.MoveNext() && otherValues.MoveNext())
        {
            if (thisValues.Current is null ^ otherValues.Current is null)
            {
                return false;
            }

            if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
            {
                return false;
            }
        }

        return !thisValues.MoveNext() && !otherValues.MoveNext();
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents().Select(x => x != null ? x.GetHashCode() : 0).Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right) => !(left == right);
}