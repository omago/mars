using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class WorkDonesRepository : Repository<WorkDone>, IWorkDonesRepository
    {
        public WorkDonesRepository(ObjectContext context): base(context)
        {

        }

        public WorkDone GetWorkDoneByPK(int workDonePK)
        {
            WorkDone a = GetAll().Where(workDone => workDone.WorkDonePK == workDonePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<WorkDone> GetWorkDonesCreatedWithToDo(int toDoListPK)
        {
            return GetValid().Where(workDone => workDone.ToDoListFK == toDoListPK && workDone.CreatedWithToDo == true);
        }

        public IQueryable<WorkDone> GetValid()
        {
           return GetAll().Where(workDone => workDone.Deleted == false || workDone.Deleted == null);
        }
    }
}
