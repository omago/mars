using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ISystemActionsRepository : IRepository<SystemAction>
    {
        SystemAction GetSystemActionByNameAndControllerFK(string Name, int SystemControllerFK);

        IQueryable<SystemAction> GetValid();
    }
}