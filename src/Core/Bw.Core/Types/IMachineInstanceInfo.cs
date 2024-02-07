namespace Bw.Core.Types;

public interface IMachineInstanceInfo
{
    string ClientGroup { get; }
    Guid ClientId { get; }
}


public record MachineInstanceInfo(Guid ClientId, string ClientGroup) : IMachineInstanceInfo
{
    public static MachineInstanceInfo New() => new(Guid.NewGuid(), AppDomain.CurrentDomain.FriendlyName);
}