using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAdditionalFactorsRepository : IRepository<AdditionalFactor>
    {
        AdditionalFactor GetAdditionalFactorByPK(int additionalFactorPK);

        IQueryable<AdditionalFactor> GetValid();
    }
}