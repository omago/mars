using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ILegalEntityLegalRepresentativesRepository : IRepository<LegalEntityLegalRepresentative>
    {
        LegalEntityLegalRepresentative GetLegalEntityLegalRepresentativeByPK(int legalEntityLegalRepresentativePK);

        IQueryable<LegalEntityLegalRepresentative> GetValid();
    }
}
