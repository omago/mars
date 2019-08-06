using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IWorkDonesRepository : IRepository<WorkDone>
    {
        WorkDone GetWorkDoneByPK(int workDonePK);

        IQueryable<WorkDone> GetWorkDonesCreatedWithToDo(int toDoListPK);
        IQueryable<WorkDone> GetValid();
    }
}