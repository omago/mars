using System;
using System.Web;
using System.Web.Mvc;
using PITFramework.Security;
using Mateus.Model.Account;
using System.Web.Security;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.BussinesLogic.Views.UserActivityModel;
using System.Web.Configuration;

namespace Mateus.Controllers
{
    public class AccountController : Controller
    {
        Mateus_wcEntities db = null;

        // GET: /Account/
        public AccountController()
        {
            db = new Mateus_wcEntities();
        }

        public ActionResult Index()
        {
            return View("LogOn");
        }


        // POST: /Account/LogOn
        [HttpPost]
        public ActionResult LogOn(LogOn model, string returnUrl)
        {
            IUsersRepository usersRepository = new UsersRepository(db);
            IRolesRepository rolesRepository = new RolesRepository(db);

            if (ModelState.IsValid)
            {
                if (usersRepository.ValidateUser(model.UserName, model.Password))
                {
                    string[] roles = rolesRepository.GetRoleNamesByUsername(model.UserName);
                    string userData = String.Join(", ", roles);

                    User user = usersRepository.GetUserByUsername(model.UserName);

                    userData += "|" + user.UserPK;

                    double sessionMinutes = ((SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState")).Timeout.TotalMinutes;

                    FormsAuthenticationTicket fAuthTicket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddMinutes(sessionMinutes), model.RememberMe, userData, FormsAuthentication.FormsCookiePath);
                    string hashCookies = FormsAuthentication.Encrypt(fAuthTicket);
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashCookies);
                    Response.Cookies.Add(cookie);

                    IUserActivitiesRepository userActivitiesRepository = new UserActivitiesRepository(db);

                    UserActivity userActivity = UserActivityView.LogUserActivity(user.UserPK, "Ulazak u sustav.", DateTime.Now);

                    userActivitiesRepository.Add(userActivity);
                    userActivitiesRepository.SaveChanges();

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "ToDoList");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kriva kombinacija korisničkog imena i lozinke.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        // GET: /Account/LogOff
        public ActionResult LogOff(string returnUrl)
        {
            int userPK = SecurityHelper.GetUserPKFromCookie();

            FormsAuthentication.SignOut();

            if (Session != null)
            {
                Session.Abandon();

                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie);
            }

            IUserActivitiesRepository userActivitiesRepository = new UserActivitiesRepository(db);

            if (userPK != 0)
            {
                UserActivity userActivity = UserActivityView.LogUserActivity(userPK, "Izlazak iz sustava.", DateTime.Now);

                userActivitiesRepository.Add(userActivity);
                userActivitiesRepository.SaveChanges();
            }

            return RedirectToAction("Index", "Account", new { returnUrl = returnUrl });
        }


    }
}
