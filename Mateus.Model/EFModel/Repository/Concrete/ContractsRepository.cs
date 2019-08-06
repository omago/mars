using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class ContractsRepository : Repository<Contract>, IContractsRepository
    {
        public ContractsRepository(ObjectContext context): base(context)
        {

        }

        public Contract GetContractByPK(int contractPK)
        {
            Contract a = GetAll().Where(contract => contract.ContractPK == contractPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<Contract> GetValid()
        {
           return GetAll().Where(contract => contract.Deleted == false || contract.Deleted == null);
        }
    }
}
