using System.Reflection;
using BitWrite.Cqrs.Command;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace BitWrite.Core.Cqrs;

public static class Extensions
{
    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Assembly[] assemblies
    )
    {
        services.Scan(scan =>
        {
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });

        services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
        services.AddScoped<ICommandBus, DefaultCommandBus>();

        return services;
    }
}

public static class ServiceTypeSelectorExtensions
{
    public static ILifetimeSelector AsClosedTypeOf(this IServiceTypeSelector selector, Type closeType)
    {
        return _ = selector.As(t =>
        {
            var types = t.GetInterfaces()
                .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == closeType)
                .Select(
                    implementedInterface =>
                        implementedInterface.GenericTypeArguments.Any(x => x.IsTypeDefinition)
                            ? implementedInterface
                            : implementedInterface.GetGenericTypeDefinition()
                )
                .Distinct();
            var result = types.ToList();
            return result;
        });
    }
}