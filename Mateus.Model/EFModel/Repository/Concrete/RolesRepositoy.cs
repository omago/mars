using System;
using System.Collections.Generic;
using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class RolesRepository : Repository<Role>, IRolesRepository
    {
        IUsersInRolesRepository usersInRolesRepository;
        IUsersRepository usersRespository;

        public RolesRepository(ObjectContext context) 
            : base(context)
        {
            usersInRolesRepository = new UsersInRolesRepository(context);
            usersRespository = new UsersRepository(context);
        }

        public bool RoleExists(Role role)
        {
            bool roleExists = GetAll().Where(r => r.Name == role.Name).Count() == 1 ? true : false;

            return roleExists;
        }

        public bool RoleExists(string roleName)
        {
            bool roleExists = GetAll().Where(role => role.Name == roleName).Count() == 1 ? true : false;

            return roleExists;
        }

        public bool IsUserInRole(User user, Role role)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public string[] GetRoleNamesByUser(User user)
        {
            throw new NotImplementedException();
        }

        public string[] GetRoleNamesByUsername(string username)
        {
            var users = usersRespository.GetAll();
            var usersInRoles = usersInRolesRepository.GetAll();
            var roles = GetAll();

            var resultQuery = (from uir in usersInRoles
                               from u in users.Where(user => user.UserPK == uir.UserFK).DefaultIfEmpty()
                               from r in roles.Where(role => role.RolePK == uir.RoleFK).DefaultIfEmpty()
                               where u.Username == username
                               select new {roleName = r.Name})
                               .Select( t => t.roleName) 
                               .ToArray();

            return resultQuery;
        }

        public IQueryable<Role> GetRolesByUser(User user)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Role> GetRolesByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Role> GetValidRoles()
        {
            return GetActiveRoles().Where(role => role.Deleted == false);
        }

        public IQueryable<Role> GetActiveRoles()
        {
            return GetAll().Where(role => role.Active == true);
        }

        public IQueryable<Role> GetInactiveRoles()
        {
            return GetAll().Where(role => role.Active == false);
        }

        public IQueryable<Role> GetDeletedRoles()
        {
            return GetAll().Where(role => role.Deleted == true);
        }
    }
}
