using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IContractValiditiesRepository : IRepository<ContractValidity>
    {
        ContractValidity GetContractValidityByPK(int contractValidityPK);

        IQueryable<ContractValidity> GetValid();
    }
}
