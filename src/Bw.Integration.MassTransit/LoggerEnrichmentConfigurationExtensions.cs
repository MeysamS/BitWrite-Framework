using Bw.Logging.Enrichers;
using Serilog;
using Serilog.Configuration;

namespace Bw.Integration.MassTransit;

public static class LoggerEnrichmentConfigurationExtensions
{
    /// <summary>
    /// Enrich logger output with Baggage information from the current <see cref="Activity"/>.
    /// </summary>
    /// <param name="loggerEnrichmentConfiguration">The enrichment configuration.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration WithBaggage(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(loggerEnrichmentConfiguration);
        return loggerEnrichmentConfiguration.With(new BaggageEnricher());
    }
}
