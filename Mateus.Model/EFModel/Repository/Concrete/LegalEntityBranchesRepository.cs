using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class LegalEntityBranchesRepository : Repository<LegalEntityBranch>, ILegalEntityBranchesRepository
    {
        public LegalEntityBranchesRepository(ObjectContext context): base(context)
        {

        }

        public LegalEntityBranch GetLegalEntityBranchByPK(int legalEntityBranchPK)
        {
            LegalEntityBranch a = GetAll().Where(legalEntityBranch => legalEntityBranch.LegalEntityBranchPK == legalEntityBranchPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<LegalEntityBranch> GetValid()
        {
           return GetAll().Where(legalEntityBranch => legalEntityBranch.Deleted == false || legalEntityBranch.Deleted == null);
        }

        public IQueryable<LegalEntityBranch> GetLegalEntityBranchesByLegalEntity(int? legalEntityPK)
        {
            return GetValid().Where(legalEntityBranch => legalEntityBranch.LegalEntityFK == legalEntityPK);
        }
    }
}
