namespace Bw.XCommonHttpclient.Models;

public class GetRequestModel : RequestModel
{
    public Dictionary<string, string>? QueryStrings { get; set; }
}
