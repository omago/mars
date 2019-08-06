using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class LegalEntityOwnersRepository : Repository<LegalEntityOwner>, ILegalEntityOwnersRepository
    {
        public LegalEntityOwnersRepository(ObjectContext context): base(context)
        {

        }

        public LegalEntityOwner GetLegalEntityOwnerByPK(int ownerPK)
        {
            LegalEntityOwner a = GetAll().Where(owner => owner.LegalEntityOwnerPK == ownerPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<LegalEntityOwner> GetValid()
        {
           return GetAll().Where(legalEntityOwner => legalEntityOwner.Deleted == false || legalEntityOwner.Deleted == null);
        }

        public IQueryable<LegalEntityOwner> GetFirstLegalEntityOwnersForLegalEntity(int legalEntityFK)
        {
            return GetValid().Where(owner => owner.LegalEntityFK == legalEntityFK);
        }
    }
}
