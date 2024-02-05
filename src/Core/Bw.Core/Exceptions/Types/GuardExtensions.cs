using Ardalis.GuardClauses;
using System.Text.RegularExpressions;

namespace Bw.Core.Exceptions.Types;

public static class GuardExtensions
{
    private static readonly Regex _regex =
        new(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))"
                + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.Compiled
        );

    private static readonly HashSet<string> _allowedCurrency = new() { "USD", "EUR", };

    public static T Null<T>(this IGuardClause guardClause, T input, Exception exception)
    {
        if (input is null)
        {
            throw exception;
        }

        return input;
    }

    public static string NullOrEmpty(this IGuardClause guardClause, string input, Exception exception)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw exception;
        }

        return input;
    }

    public static string NullOrWhiteSpace(this IGuardClause guardClause, string input, Exception exception)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw exception;
        }

        return input;
    }

    public static decimal NegativeOrZero(this IGuardClause guardClause, decimal input, Exception exception)
    {
        return guardClause.NegativeOrZero<decimal>(input, exception);
    }

    public static int NegativeOrZero(this IGuardClause guardClause, int input, Exception exception)
    {
        return guardClause.NegativeOrZero<int>(input, exception);
    }

    public static long NegativeOrZero(this IGuardClause guardClause, long input, Exception exception)
    {
        return guardClause.NegativeOrZero<long>(input, exception);
    }

    public static double NegativeOrZero(this IGuardClause guardClause, double input, Exception exception)
    {
        return guardClause.NegativeOrZero<double>(input, exception);
    }

    private static T NegativeOrZero<T>(this IGuardClause guardClause, T input, Exception exception)
        where T : struct, IComparable
    {
        if (input.CompareTo(default(T)) <= 0)
        {
            throw exception;
        }

        return input;
    }

    public static decimal Negative(this IGuardClause guardClause, decimal input, Exception exception)
    {
        return guardClause.Negative<decimal>(input, exception);
    }

    public static int Negative(this IGuardClause guardClause, int input, Exception exception)
    {
        return guardClause.Negative<int>(input, exception);
    }

    public static long Negative(this IGuardClause guardClause, long input, Exception exception)
    {
        return guardClause.Negative<long>(input, exception);
    }

    public static double Negative(this IGuardClause guardClause, double input, Exception exception)
    {
        return guardClause.Negative<double>(input, exception);
    }

    private static T Negative<T>(this IGuardClause guardClause, T input, Exception exception)
        where T : struct, IComparable
    {
        if (input.CompareTo(default(T)) < 0)
        {
            throw exception;
        }

        return input;
    }

    public static bool NotExists(this IGuardClause guardClause, bool input, Exception exception)
    {
        if (input == false)
        {
            throw exception;
        }

        return input;
    }

    public static T NotFound<T>(this IGuardClause guardClause, T input, Exception exception)
    {
        if (input is null)
        {
            throw exception;
        }

        return input;
    }

    public static DateTime InvalidDate(this IGuardClause guardClause, DateTime date)
    {
        if (date == default)
        {
            throw new InvalidDateException(date);
        }

        return date;
    }

    public static string InvalidEmail(this IGuardClause guardClause, string email, Exception exception)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw exception;
        }

        if (email.Length > 100)
        {
            throw exception;
        }

        var lowerEmail = email.ToLowerInvariant();
        if (!_regex.IsMatch(lowerEmail))
        {
            throw exception;
        }

        return email;
    }

    public static string InvalidCurrency(this IGuardClause guardClause, string currency)
    {
        return guardClause.InvalidCurrency(currency, new InvalidCurrencyException(currency));
    }

    public static string InvalidCurrency(this IGuardClause guardClause, string? currency, Exception exception)
    {
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
        {
            throw exception;
        }

        currency = currency.ToUpperInvariant();
        if (!_allowedCurrency.Contains(currency))
        {
            throw exception;
        }

        return currency;
    }

}
