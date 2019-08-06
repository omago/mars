using System;
using System.Linq;
using System.Data.Objects;
using PITFramework.Repository;
using PITFramework.Security;
using Mateus.Model.EFModel.Repository.Interface;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class UsersRepository : Repository<User>, IUsersRepository
    {
         public UsersRepository(ObjectContext context) 
             : base(context) 
         {

         }

         #region IUsersRepository

         public bool UserExists(User user)
         {
             return GetAll().Contains(user);
         }

         public bool UserExists(string username)
         {
             bool userExists = GetAll().Where(user => user.Username == username).Count() == 1 ? true : false;

             return userExists;
         }

         public string[] GetUsernamesInRole(Role role)
         {
             throw new NotImplementedException();
         }

         public string[] GetUsernamesByRoleName(string roleName)
         {
             throw new NotImplementedException();
         }

         public IQueryable<User> GetUsersInRole(User user, Role role)
         {
             throw new NotImplementedException();
         }

         public IQueryable<User> GetUsersByRoleName(string roleName)
         {
             throw new NotImplementedException();
         }

         public User GetActiveUserByUsername(string username)
         {
             var u = GetAll().Where(user => user.Username == username && user.Active == true);

             if (u.Count() > 0)
             {
                 return u.First();
             }
             else 
             { 
                 return null; 
             }
         }

         public User GetUserByUsername(string username)
         {
             var u = GetAll().Where(user => user.Username == username);

             if (u.Count() > 0)
             {
                 return u.First();
             }
             else 
             { 
                 return null; 
             }
         }

         public User GetUserByEmail(string email)
         {
             var u = GetAll().Where(user => user.Email == email);

             if (u.Count() > 0)
             {
                 return u.First();
             }
             else
             {
                 return null;
             }
         }

         public IQueryable<User> GetActiveUsers()
         {
             return GetAll().Where(user => user.Active == true);
         }

         public IQueryable<User> GetInactiveUsers()
         {
             return GetAll().Where(user => user.Active == false);
         }

         public IQueryable<User> GetDeletedUsers()
         {
             return GetAll().Where(user => user.Deleted == true);
         }

         public bool ValidateUser(string username, string password) {
             User user = GetActiveUserByUsername(username);

             if (user != null)
             {
                 PBKDF2 pbkdf2 = new PBKDF2(password, Convert.FromBase64String(user.Salt));

                 bool passwordVerified = pbkdf2.VerifyPassword(user.Password);

                 if (passwordVerified)
                 {
                     return true; // User validated succesfully
                 }
                 else
                 {
                     return false; // Wrong password
                 }
             }
             else
             {
                 return false; // User with specified "username" doesn't exists
             }
         }

         #endregion

         public User GetUserByUserID(int UserID)
         {
             var u = GetAll().Where(user => user.UserPK == UserID);

             if (u.Count() > 0)
             {
                 return u.First();
             }
             else
             {
                 return null;
             }
         }

         public IQueryable<User> GetValid()
         {
             return GetAll().Where( u => u.Deleted == false || u.Deleted == null);
         }
    }
}
