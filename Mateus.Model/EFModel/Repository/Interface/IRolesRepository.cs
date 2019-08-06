using System.Collections.Generic;
using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IRolesRepository: IRepository<Role>
    {
        bool RoleExists(Role role);
        bool RoleExists(string roleName);

        bool IsUserInRole(User user, Role role);
        bool IsUserInRole(string username, string roleName);

        string[] GetRoleNamesByUser(User user);
        string[] GetRoleNamesByUsername(string username);
        IQueryable<Role> GetRolesByUser(User user);
        IQueryable<Role> GetRolesByUsername(string username);

        IEnumerable<Role> GetValidRoles();
        IQueryable<Role> GetActiveRoles();
        IQueryable<Role> GetInactiveRoles();
        IQueryable<Role> GetDeletedRoles();
    }
}
