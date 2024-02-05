namespace Bw.Core.Domain.Model.Identity;

public interface IIdentity<out TId>
{
    public TId Value { get; }
}
