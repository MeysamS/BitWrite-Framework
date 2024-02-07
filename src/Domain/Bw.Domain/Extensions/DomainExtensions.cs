using Bw.Core.Domain.Event;
using Bw.Domain.Event;
using Microsoft.Extensions.DependencyInjection;

namespace Bw.Domain.Extensions;

public static class DomainExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        return services;
    }


}
