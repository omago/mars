using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class ContractValiditiesRepository : Repository<ContractValidity>, IContractValiditiesRepository
    {
        public ContractValiditiesRepository(ObjectContext context): base(context)
        {

        }

        public ContractValidity GetContractValidityByPK(int contractValidityPK)
        {
            ContractValidity a = GetAll().Where(contractValidity => contractValidity.ContractValidityPK == contractValidityPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<ContractValidity> GetValid()
        {
           return GetAll().Where(contractValidity => contractValidity.Deleted == false || contractValidity.Deleted == null);
        }
    }
}
