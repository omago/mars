using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IToDoListsRepository : IRepository<ToDoList>
    {
        ToDoList GetToDoListByPK(int toDoListPK);

        IQueryable<ToDoList> GetValid();
        IQueryable<ToDoList> GetNotFinished();
    }
}
