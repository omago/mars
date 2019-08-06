using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class LegalEntityBanksRepository : Repository<LegalEntityBank>, ILegalEntityBanksRepository
    {
        public LegalEntityBanksRepository(ObjectContext context): base(context)
        {

        }

        public LegalEntityBank GetLegalEntityBankByPK(int legalEntityBankPK)
        {
            LegalEntityBank a = GetAll().Where(legalEntityBank => legalEntityBank.LegalEntityBankPK == legalEntityBankPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<LegalEntityBank> GetValid()
        {
           return GetAll().Where(legalEntityBank => legalEntityBank.Deleted == false || legalEntityBank.Deleted == null);
        }
    }
}
