namespace Bw.Persistence.EventStore.Abstraction;

public record AppendResult(long GlobalPosition, long NextExpectedVersion)
{
    public static readonly AppendResult None = new(0, -1);
};


