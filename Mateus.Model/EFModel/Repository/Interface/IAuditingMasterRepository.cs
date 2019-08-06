using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAuditingMasterRepository : IRepository<AuditingMaster>
    {
         AuditingMaster GetAuditingMasterByPK(int auditingMasterPK);
    }
}
