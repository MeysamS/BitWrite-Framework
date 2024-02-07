namespace Bw.XCommonHttpclient.Polly.PollyOptions;

public class PolicyConfig : ICircuitBreakerPolicyConfig, IRetryPolicyConfig
{
    public int RetryCount { get; set; }
    public int BreakDuration { get; set; }
}
