using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Principal;
using Mateus.Controllers;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;

namespace Mateus.Support
{
    public class PITAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private string _roles;
        private string[] _rolesSplit = new string[0];
        private bool isAuthorized = false;

        public string Roles
        {
            get
            {
                return _roles ?? String.Empty;
            }
            set
            {
                _roles = value;
                _rolesSplit = StringHelper.SplitString(value);
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            filterContext.HttpContext.Response.Cache.SetNoServerCaching();
            filterContext.HttpContext.Response.Cache.SetNoStore();

            IPrincipal user = filterContext.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                // authentication failed, redirect to login page 
                string returnUrl = "";
                if (filterContext.HttpContext.Request.Url != null)
                {
                    returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery;
                }
                filterContext.Result = new AccountController().LogOff(returnUrl);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(Roles))
                {
                    isAuthorized = true;
                }
                else
                {
                    Mateus_wcEntities db = new Mateus_wcEntities();
                    IRolesRepository rolesRepository = new RolesRepository(db);
                    var userRoles = rolesRepository.GetRoleNamesByUsername(user.Identity.Name); //rolesRepository.GetRoleNameByUsername(user.Identity.Name); // 

                    foreach (string role in userRoles)
                    {
                        if (_rolesSplit.Contains(role))
                        {
                            isAuthorized = true;
                            break;
                        }
                    }
                }

                if (!isAuthorized)
                {
                    filterContext.Result = new RedirectResult("~/Error/UnauthorizedAccess");
                }
            }
        }
    }
}