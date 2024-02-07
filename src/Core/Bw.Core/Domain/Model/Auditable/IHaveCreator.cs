namespace Bw.Core.Domain.Model.Auditable;

public interface IHaveCreator

{
    DateTime CreatedDate { get; }
    int? CreatorId { get; }
}