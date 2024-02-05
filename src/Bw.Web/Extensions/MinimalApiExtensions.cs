using Asp.Versioning.Builder;
using Bw.Core.Reflection;
using Bw.Web.MinimalApi;
using LinqKit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;


namespace Bw.Web.Extensions;
public static class MinimalApiExtensions
{
    public static IServiceCollection AddMinimalEndpoints(
        this WebApplicationBuilder applicationBuilder,
        params Assembly[] scanAssemblies
    )
    {
        // Assemblies are lazy loaded so using AppDomain.GetAssemblies is not reliable (it is possible to get ReflectionTypeLoadException, because some dependent type assembly are lazy and not loaded yet), so we use `GetAllReferencedAssemblies` and it load all referenced assemblies explicitly.
        // we also load assmblies that have some endpoints and known as a application part, because assemblies are lazy and maybe at the time of scanning, assmblies contain endpoints not visited yet.
        var assemblies = scanAssemblies.Any()
            ? scanAssemblies
            : ReflectionUtilities
                .GetReferencedAssemblies(Assembly.GetCallingAssembly())
                .Concat(ReflectionUtilities.GetApplicationPartAssemblies(Assembly.GetCallingAssembly()))
                .Distinct()
                .ToArray();

        applicationBuilder.Services.Scan(
            scan =>
                scan.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo(typeof(IMinimalEndpoint)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Append)
                    .As<IMinimalEndpoint>()
                    .WithLifetime(ServiceLifetime.Scoped)
        );

        return applicationBuilder.Services;
    }

    public static IServiceCollection AddMinimalEndpoints(
        this IServiceCollection services,
        params Assembly[] scanAssemblies
    )
    {
        // Assemblies are lazy loaded so using AppDomain.GetAssemblies is not reliable (it is possible to get ReflectionTypeLoadException, because some dependent type assembly are lazy and not loaded yet), so we use `GetAllReferencedAssemblies` and it load all referenced assemblies explicitly.
        // we also load assmblies that have some endpoints and known as a application part, because assemblies are lazy and maybe at the time of scanning, assmblies contain endpoints not visited yet.
        var assemblies = scanAssemblies.Any()
            ? scanAssemblies
            : ReflectionUtilities
                .GetReferencedAssemblies(Assembly.GetCallingAssembly())
                .Concat(ReflectionUtilities.GetApplicationPartAssemblies(Assembly.GetCallingAssembly()))
                .Distinct()
                .ToArray();

        services.Scan(
            scan =>
                scan.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo(typeof(IMinimalEndpoint)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Append)
                    .As<IMinimalEndpoint>()
                    .WithLifetime(ServiceLifetime.Scoped)
        );

        return services;
    }

    /// <summary>
    /// Map registered minimal apis.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapMinimalEndpoints(this IEndpointRouteBuilder builder)
    {
        var scope = builder.ServiceProvider.CreateScope();

        var endpoints = scope.ServiceProvider.GetServices<IMinimalEndpoint>().ToList();

        var versionGroups = endpoints
            .GroupBy(x => x.GroupName)
            .ToDictionary(x => x.Key, c =>
            {
                return builder.MapApiGroup(c.Key).WithTags(c.Key);
            });

        var versionSubGroups = endpoints
            .GroupBy(
                x =>
                    new
                    {
                        x.GroupName,
                        x.PrefixRoute,
                        x.Version
                    }
            )
            .ToDictionary(
                x => x.Key,
                c => versionGroups[c.Key.GroupName].MapGroup(c.Key.PrefixRoute).HasApiVersion(c.Key.Version)
            );

        var endpointVersions = endpoints
            .GroupBy(x => new { x.GroupName, x.Version })
            .Select(
                x =>
                    new
                    {
                        Verion = x.Key.Version,
                        x.Key.GroupName,
                        Endpoints = x.Select(v => v)
                    }
            );

        foreach (var endpointVersion in endpointVersions)
        {
            var versionGroup = versionSubGroups.FirstOrDefault(x => x.Key.GroupName == endpointVersion.GroupName).Value;

            endpointVersion.Endpoints.ForEach(ep =>
            {
                ep.MapEndpoint(versionGroup);
            });
        }

        return builder;
    }
}


public static class IEndpointRouteBuilderExtensions
{


    public static IVersionedEndpointRouteBuilder MapApiGroup(this IEndpointRouteBuilder builder, string? name = null)
    {
        if (builder == null)
        {
            throw new ArgumentNullException("builder");
        }

        if (builder.IsNestedGroup())
        {
            throw new InvalidOperationException("CannotNestApiGroup");
        }

        RouteGroupBuilder routeGroupBuilder = builder.MapGroup(string.Empty);
        return builder.NewVersionedEndpointRouteBuilder(routeGroupBuilder, routeGroupBuilder, name);
    }
    private static bool IsNestedGroup(this IEndpointRouteBuilder builder)
    {
        if (!(builder is RouteGroupBuilder))
        {
            return builder.HasMetadata();
        }

        return true;
    }

    private static bool HasMetadata(this IEndpointRouteBuilder builder)
    {
        return builder.ServiceProvider.GetService<ApiVersionSetBuilder>() != null;
    }


    private static IVersionedEndpointRouteBuilder NewVersionedEndpointRouteBuilder(this IEndpointRouteBuilder builder, IEndpointRouteBuilder routeBuilder, IEndpointConventionBuilder conventionBuilder, string? name)
    {
        VersionedEndpointRouteBuilderFactory service = builder.ServiceProvider.GetService<VersionedEndpointRouteBuilderFactory>()!;
        ApiVersionSetBuilder apiVersionSetBuilder = builder.NewApiVersionSet(name);
        if (service != null)
        {
            return service(routeBuilder, conventionBuilder, apiVersionSetBuilder);
        }

        return new VersionedEndpointRouteBuilder(routeBuilder, conventionBuilder, apiVersionSetBuilder);
    }

}