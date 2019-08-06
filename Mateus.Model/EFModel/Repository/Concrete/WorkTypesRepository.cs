using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class WorkTypesRepository : Repository<WorkType>, IWorkTypesRepository
    {
        public WorkTypesRepository(ObjectContext context): base(context)
        {

        }

        public WorkType GetWorkTypeByPK(int workTypePK)
        {
            WorkType a = GetAll().Where(workType => workType.WorkTypePK == workTypePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<WorkType> GetValid()
        {
           return GetAll().Where(workType => workType.Deleted == false || workType.Deleted == null);
        }
    }
}
