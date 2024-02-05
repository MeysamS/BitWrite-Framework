namespace Bw.XCommonHttpclient.Models;

public class PostRequestModel : RequestModel
{
    public Dictionary<string, string> BodyStrings { get; set; } = default!;
}
