using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AuditingMasterRepository : Repository<AuditingMaster>, IAuditingMasterRepository
    {
        public AuditingMasterRepository(ObjectContext context): base(context)
        {
            base.AuditRepository = false;
        }

        public AuditingMaster GetAuditingMasterByPK(int auditingMasterPK)
        {
            AuditingMaster am = GetAll().Where(auditingMaster => auditingMaster.AuditingMasterPK == auditingMasterPK).First();

            if (am != null)
            {
                return am;
            }
            else
            {
                return null;
            }
        }
    }
}
