using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class ToDoListsRepository : Repository<ToDoList>, IToDoListsRepository
    {
        public ToDoListsRepository(ObjectContext context): base(context)
        {

        }

        public ToDoList GetToDoListByPK(int toDoListPK)
        {
            ToDoList a = GetAll().Where(toDoList => toDoList.ToDoListPK == toDoListPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<ToDoList> GetValid()
        {
           return GetAll().Where(toDoList => toDoList.Deleted == false || toDoList.Deleted == null);
        }

        public IQueryable<ToDoList> GetNotFinished()
        {
           return GetValid().Where(toDoList => toDoList.Finished == false || toDoList.Finished == null);
        }
    }
}
