using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class LegalEntityLegalRepresentativesRepository : Repository<LegalEntityLegalRepresentative>, ILegalEntityLegalRepresentativesRepository
    {
        public LegalEntityLegalRepresentativesRepository(ObjectContext context): base(context)
        {

        }

        public LegalEntityLegalRepresentative GetLegalEntityLegalRepresentativeByPK(int legalEntityLegalRepresentativePK)
        {
            LegalEntityLegalRepresentative a = GetAll().Where(legalEntityLegalRepresentative => legalEntityLegalRepresentative.LegalEntityLegalRepresentativePK == legalEntityLegalRepresentativePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<LegalEntityLegalRepresentative> GetValid()
        {
           return GetAll().Where(legalEntityLegalRepresentative => legalEntityLegalRepresentative.Deleted == false || legalEntityLegalRepresentative.Deleted == null);
        }
    }
}
