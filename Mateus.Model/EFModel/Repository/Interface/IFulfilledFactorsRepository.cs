using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IFulfilledFactorsRepository : IRepository<FulfilledFactor>
    {
        FulfilledFactor GetFulfilledFactorByPK(int fulfilledFactorPK);

        IQueryable<FulfilledFactor> GetValid();
    }
}