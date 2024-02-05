using Scrutor;

namespace Bw.Extensions.Microsoft.DependencyInjection;

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