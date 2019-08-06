using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAuditingDetailsRepository : IRepository<AuditingDetail>
    {
        AuditingDetail GetAuditingDetailsByPK(int auditingDetailPK);

        IQueryable<AuditingDetail> GetModifiedPropertiesOnChangeDate(int? RelatedEntityPK, string TableName);
        IQueryable<AuditingDetail> GetPropertiesForTableName(string TableName);
        IQueryable<AuditingDetail> GetAuditingDetailByAuditingMasterPK(int? auditingMasterPK);
    }
}