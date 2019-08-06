using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IWorkTypesRepository : IRepository<WorkType>
    {
        WorkType GetWorkTypeByPK(int workTypePK);

        IQueryable<WorkType> GetValid();
    }
}
