using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAnnexContractsRepository : IRepository<AnnexContract>
    {
        AnnexContract GetAnnexContractByPK(int annexContractPK);

        IQueryable<AnnexContract> GetValid();
    }
}
