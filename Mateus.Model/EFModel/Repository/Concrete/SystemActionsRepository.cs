using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class SystemActionsRepository : Repository<SystemAction>, ISystemActionsRepository
    {
        public SystemActionsRepository(ObjectContext context): base(context)
        {

        }

        public SystemAction GetSystemActionByNameAndControllerFK(string Name, int SystemControllerFK)
        {
            var a = GetValid().Where(action => action.Name == Name && action.SystemControllerFK == SystemControllerFK);

            if (a != null && a.Count() > 0)
            {
                return a.First();
            }
            else
            {
                return null;
            }
        }

        public IQueryable<SystemAction> GetValid()
        {
           return GetAll().Where(action => action.Deleted == false || action.Deleted == null);
        }
    }
}
