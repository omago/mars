using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IUsersRepository: IRepository<User>
    {
        bool UserExists(User user);
        bool UserExists(string username);

        User GetUserByUsername(string username);
        User GetUserByUserID(int UserID);
        User GetUserByEmail(string email);
        IQueryable<User> GetActiveUsers();
        IQueryable<User> GetInactiveUsers();
        IQueryable<User> GetDeletedUsers();

        IQueryable<User> GetValid();

        string[] GetUsernamesInRole(Role role);
        string[] GetUsernamesByRoleName(string roleName);
        IQueryable<User> GetUsersInRole(User user, Role role);
        IQueryable<User> GetUsersByRoleName(string roleName);

        bool ValidateUser(string username, string password);
    }
}
