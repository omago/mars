using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AnnexContractsRepository : Repository<AnnexContract>, IAnnexContractsRepository
    {
        public AnnexContractsRepository(ObjectContext context): base(context)
        {

        }

        public AnnexContract GetAnnexContractByPK(int annexContractPK)
        {
            AnnexContract a = GetAll().Where(annexContract => annexContract.AnnexContractPK == annexContractPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<AnnexContract> GetValid()
        {
           return GetAll().Where(annexContract => annexContract.Deleted == false || annexContract.Deleted == null);
        }
    }
}
