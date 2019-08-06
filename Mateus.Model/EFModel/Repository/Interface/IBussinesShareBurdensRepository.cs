using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IBussinesShareBurdensRepository : IRepository<BussinesShareBurden>
    {
        BussinesShareBurden GetBussinesShareBurdenByPK(int bussinesShareBurdenPK);

        IQueryable<BussinesShareBurden> GetValid();
    }
}