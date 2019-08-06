using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ILegalEntityBanksRepository : IRepository<LegalEntityBank>
    {
        LegalEntityBank GetLegalEntityBankByPK(int legalEntityBankPK);

        IQueryable<LegalEntityBank> GetValid();
    }
}
