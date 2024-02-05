using Bw.Web.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Bw.Web.Extensions;

public static class MiddlewaresExtensions
{
    public static IApplicationBuilder UseRequestLogContextMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLogContextMiddleware>();
    }
}
