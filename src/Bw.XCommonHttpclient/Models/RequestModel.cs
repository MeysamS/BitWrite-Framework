namespace Bw.XCommonHttpclient.Models;

public abstract class RequestModel
{
    public string Url { get; set; } = default!;
    public Dictionary<string, string> HeaderStrings { get; set; } = default!;
}
