using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AuditingDetailsRepository : Repository<AuditingDetail>, IAuditingDetailsRepository
    {
        IAuditingMasterRepository auditingMasterRepository; 
        public AuditingDetailsRepository(ObjectContext context)
            : base(context)
        {
            auditingMasterRepository = new AuditingMasterRepository(context);
            base.AuditRepository = false;
        }

        public AuditingDetail GetAuditingDetailsByPK(int auditingDetailPK)
        {
            AuditingDetail ad = GetAll().Where(auditingDetail => auditingDetail.AuditingDetailPK == auditingDetailPK).First();

            if (ad != null)
            {
                return ad;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<AuditingDetail> GetModifiedPropertiesOnChangeDate(int? RelatedEntityPK, string TableName)
        {
            var auditingMasterAll = auditingMasterRepository.GetAll();
            var auditingDetaillAll = GetPropertiesForTableName(TableName);

            var result = (from ad in auditingDetaillAll
                          from am in auditingMasterAll.Where(am1 => am1.AuditingMasterPK == ad.AuditingMasterFK).DefaultIfEmpty()
                          select ad);

            return null;
        }

        public IQueryable<AuditingDetail> GetPropertiesForTableName(string TableName)
        {
            var auditingMasterAll = auditingMasterRepository.GetAll();
            var auditingDetaillAll = GetAll();

            var result = (from ad in auditingDetaillAll
                          from am in auditingMasterAll.Where(am1 => am1.TableName == TableName).DefaultIfEmpty()
                          select ad);

            return result;
        }

        public IQueryable<AuditingDetail> GetAuditingDetailByAuditingMasterPK(int? auditingMasterPK)
        {
            return GetAll().Where(ad => ad.AuditingMasterFK == auditingMasterPK);
        }
    }
}
