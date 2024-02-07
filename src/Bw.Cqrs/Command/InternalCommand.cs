using Bw.Core.Cqrs.Commands;
using Bw.Core.Utils;

namespace Bw.Cqrs.Command;

public abstract record InternalCommand : IInternalCommand
{
    public Guid InternalCommandId { get; protected set; } = Guid.NewGuid();

    public DateTime OccurredOn { get; protected set; } = DateTime.Now;

    public string Type { get { return TypeMapper.GetFullTypeName(GetType()); } }
}