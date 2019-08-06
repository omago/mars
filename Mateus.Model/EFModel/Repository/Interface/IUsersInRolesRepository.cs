using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IUsersInRolesRepository : IRepository<UserInRole>
    {
        IQueryable<UserInRole> GetRolesByUserPK(int userFK);
    }
}
