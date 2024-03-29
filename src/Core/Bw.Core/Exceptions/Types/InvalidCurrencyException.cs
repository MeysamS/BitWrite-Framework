namespace Bw.Core.Exceptions.Types;

public class InvalidCurrencyException : BadRequestException
{
    public string Currency { get; }

    public InvalidCurrencyException(string currency)
        : base($"Currency: '{currency}' is invalid.")
    {
        Currency = currency;
    }
}