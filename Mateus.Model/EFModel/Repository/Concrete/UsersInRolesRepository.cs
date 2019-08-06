using System.Linq;
using System.Data.Objects;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class UsersInRolesRepository : Repository<UserInRole>, IUsersInRolesRepository
    {
        public UsersInRolesRepository(ObjectContext context) 
             : base(context) 
         {

         }

        public IQueryable<UserInRole> GetRolesByUserPK(int userFK)
        {
            return GetAll().Where(userInRole => userInRole.UserFK == userFK);
        }
    }
}
