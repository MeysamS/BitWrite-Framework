using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bw.Core.Web.Module
{
    public interface IModuleConfiguration
    {
        WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder);

        Task<WebApplication> ConfigureModule(WebApplication app);

        IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
    }
}