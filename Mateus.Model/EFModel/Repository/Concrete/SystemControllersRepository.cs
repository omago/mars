using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class SystemControllersRepository : Repository<SystemController>, ISystemControllersRepository
    {
        public SystemControllersRepository(ObjectContext context): base(context)
        {

        }

        public SystemController GetSystemControllerByName(string Name)
        {
            var sc = GetValid().Where(systemController => systemController.Name == Name);

            if (sc != null && sc.Count() > 0)
            {
                return sc.First();
            }
            else
            {
                return null;
            }
        }

        public IQueryable<SystemController> GetValid()
        {
           return GetAll().Where(systemController => systemController.Deleted == false || systemController.Deleted == null);
        }
    }
}
