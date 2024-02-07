namespace Bw.Core.Domain.Model.Auditable;


public interface IHaveAudit : IHaveCreator
{
    DateTime? UpdatedDate { get; }
    int? UpdatorId { get; }
}
