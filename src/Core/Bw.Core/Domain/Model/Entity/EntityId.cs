using Bw.Core.Domain.Model.Identity;

namespace Bw.Core.Domain.Model.Entity;

public record EntityId<T> : Identity<T>
{
    public static EntityId<T> CreateEntityId(T id) => new() { Value = id };
}

public record EntityId : EntityId<long>
{
    public static new EntityId CreateEntityId(long id) => new() { Value = id };
}