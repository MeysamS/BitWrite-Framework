namespace Bw.XCommonHttpclient.Polly.PollyOptions;

public interface IRetryPolicyConfig
{
    int RetryCount { get; set; }
}
