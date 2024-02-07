using Bw.Core.Domain.Event.Internal;
using Bw.Core.Reflection.Extensions;
using System.Reflection;

namespace Bw.Domain.Extensions;

public static class TypeExtensions
{
    public static IReadOnlyDictionary<Type, Action<TDomainEvent>> GetAggregateApplyMethods<TDomainEvent>(this Type type)
        where TDomainEvent : IDomainEvent
    {
        var aggregateEventType = typeof(TDomainEvent);

        return type.GetTypeInfo()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(mi =>
            {
                if (
                    !string.Equals(mi.Name, "Apply", StringComparison.Ordinal)
                    && !mi.Name.EndsWith(".Apply", StringComparison.Ordinal)
                )
                {
                    return false;
                }

                var parameters = mi.GetParameters();
                return parameters.Length == 1
                    && aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
            })
            .ToDictionary(
                mi => mi.GetParameters()[0].ParameterType,
                mi => type.CompileMethodInvocation<Action<TDomainEvent>>(mi.Name, mi.GetParameters()[0].ParameterType)
            );
    }
}
