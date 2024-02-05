namespace Bw.Core.Domain.Model;

public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}