namespace Bw.XCommonHttpclient.Polly.PollyOptions;

public class UsePollyConfig
{
    public bool Active { get; set; }
    public PolicyConfig PolicyConfig { get; set; } = default!;
    public Action<PolicyConfig>? PolicyConfigAction { get; set; }
}
