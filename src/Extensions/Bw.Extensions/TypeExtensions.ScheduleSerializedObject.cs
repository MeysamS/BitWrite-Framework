using Bw.Core.Scheduling;
using System.Reflection;

namespace Bw.Extensions;


public static class TypeExtensions
{
    public static Type? GetPayloadType(this ScheduleSerializedObject messageSerializedObject)
    {
        if (messageSerializedObject.AssemblyName == null)
            return null;

        var assembly = Assembly.Load(messageSerializedObject.AssemblyName);

        var type = assembly
            .GetTypes()
            .Where(t => t.FullName == messageSerializedObject.FullTypeName)
            .ToList().FirstOrDefault();
        return type;
    }
}
