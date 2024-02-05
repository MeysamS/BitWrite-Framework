using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bw.Core.Web.Module
{
    public interface ISharedModulesConfiguration
    {
        WebApplicationBuilder AddSharedModuleServices(WebApplicationBuilder builder);

        Task<WebApplication> ConfigureSharedModule(WebApplication app);

        IEndpointRouteBuilder MapSharedModuleEndpoints(IEndpointRouteBuilder endpoints);
    }
}