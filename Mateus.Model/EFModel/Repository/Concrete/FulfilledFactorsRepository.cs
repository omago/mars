using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class FulfilledFactorsRepository : Repository<FulfilledFactor>, IFulfilledFactorsRepository
    {
        public FulfilledFactorsRepository(ObjectContext context): base(context)
        {

        }

        public FulfilledFactor GetFulfilledFactorByPK(int fulfilledFactorPK)
        {
            FulfilledFactor a = GetAll().Where(fulfilledFactor => fulfilledFactor.FulfilledFactorPK == fulfilledFactorPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<FulfilledFactor> GetValid()
        {
           return GetAll().Where(fulfilledFactor => fulfilledFactor.Deleted == false || fulfilledFactor.Deleted == null);
        }
    }
}
