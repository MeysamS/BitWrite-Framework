namespace Bw.Extensions.Collection;

public static class DateTimeExtensions
{
    private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long ToUnixTimeSecond(this DateTime datetime)
    {
        var unixTime = (datetime.ToUniversalTime() - Epoch).TotalSeconds;
        return (long)unixTime;
    }

    public static DateTime ToDateTime(this long? unixTime) =>
        Epoch.AddSeconds(unixTime ?? DateTime.Now.ToUnixTimeSecond()).ToLocalTime();

    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }
}