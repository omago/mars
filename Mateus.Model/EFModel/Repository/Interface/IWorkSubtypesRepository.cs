using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IWorkSubtypesRepository : IRepository<WorkSubtype>
    {
        WorkSubtype GetWorkSubtypeByPK(int workSubtypePK);

        IQueryable<WorkSubtype> GetValid();

        IQueryable<WorkSubtype> GetValidByWorkType(int workTypeFK);
    }
}
