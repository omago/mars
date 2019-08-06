using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ICommercialCourtsRepository : IRepository<CommercialCourt>
    {
        CommercialCourt GetCommercialCourtByPK(int commercialCourtPK);

        IQueryable<CommercialCourt> GetValid();
    }
}
