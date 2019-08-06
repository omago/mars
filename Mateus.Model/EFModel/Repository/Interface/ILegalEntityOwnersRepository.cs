using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ILegalEntityOwnersRepository : IRepository<LegalEntityOwner>
    {
        LegalEntityOwner GetLegalEntityOwnerByPK(int ownerPK);

        IQueryable<LegalEntityOwner> GetValid();

        IQueryable<LegalEntityOwner> GetFirstLegalEntityOwnersForLegalEntity(int legalEntityFK);
    }
}
