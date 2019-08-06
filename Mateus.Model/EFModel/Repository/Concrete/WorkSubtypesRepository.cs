using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class WorkSubtypesRepository : Repository<WorkSubtype>, IWorkSubtypesRepository
    {
        public WorkSubtypesRepository(ObjectContext context): base(context)
        {

        }

        public WorkSubtype GetWorkSubtypeByPK(int workSubtypePK)
        {
            WorkSubtype a = GetAll().Where(workSubtype => workSubtype.WorkSubtypePK == workSubtypePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<WorkSubtype> GetValid()
        {
           return GetAll().Where(workSubtype => workSubtype.Deleted == false || workSubtype.Deleted == null);
        }

        public IQueryable<WorkSubtype> GetValidByWorkType(int workTypeFK)
        {
            return  GetValid().Where(p => p.WorkTypeFK == workTypeFK); 
        }
    }
}
