using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ILegalEntityBranchesRepository : IRepository<LegalEntityBranch>
    {
        LegalEntityBranch GetLegalEntityBranchByPK(int legalEntityLegalEntityBranchPK);

        IQueryable<LegalEntityBranch> GetValid();
        IQueryable<LegalEntityBranch> GetLegalEntityBranchesByLegalEntity(int? legalEntityPK);

    }
}
