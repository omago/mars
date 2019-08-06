using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AdditionalFactorsRepository : Repository<AdditionalFactor>, IAdditionalFactorsRepository
    {
        public AdditionalFactorsRepository(ObjectContext context): base(context)
        {

        }

        public AdditionalFactor GetAdditionalFactorByPK(int additionalFactorPK)
        {
            AdditionalFactor a = GetAll().Where(additionalFactor => additionalFactor.AdditionalFactorPK == additionalFactorPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<AdditionalFactor> GetValid()
        {
           return GetAll().Where(additionalFactor => additionalFactor.Deleted == false || additionalFactor.Deleted == null);
        }
    }
}
