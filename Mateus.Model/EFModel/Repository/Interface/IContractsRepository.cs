using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IContractsRepository : IRepository<Contract>
    {
        Contract GetContractByPK(int contractPK);

        IQueryable<Contract> GetValid();
    }
}
