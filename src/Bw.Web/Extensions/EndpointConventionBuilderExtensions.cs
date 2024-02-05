using System.Globalization;
using Bw.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Bw.Web.Extensions;


public static class EndpointConventionBuilderExtensions
{
    public static RouteHandlerBuilder Produces(this RouteHandlerBuilder builder, int statusCode, string description)
    {
        builder.WithOpenApi(operation =>
        {
            operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
            return operation;
        });
        builder.Produces(statusCode);
        return builder;
    }

    public static RouteHandlerBuilder Produces<TResponse>(
        this RouteHandlerBuilder builder,
        int statusCode,
        string description
    )
    {
        builder.WithOpenApi(operation =>
        {
            operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
            return operation;
        });
        builder.Produces<TResponse>(statusCode);

        return builder;
    }
}
