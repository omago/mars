using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ISystemControllersRepository : IRepository<SystemController>
    {
        SystemController GetSystemControllerByName(string Name);

        IQueryable<SystemController> GetValid();
    }
}
