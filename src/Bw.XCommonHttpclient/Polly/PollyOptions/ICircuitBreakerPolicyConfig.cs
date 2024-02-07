namespace Bw.XCommonHttpclient.Polly.PollyOptions;

public interface ICircuitBreakerPolicyConfig
{
    int RetryCount { get; set; }
    int BreakDuration { get; set; }
}
