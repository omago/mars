using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.Account;
using Mateus.Support;
using PITFramework.Support.Grid;
using PITFramework.Security;
using PITFramework.Auditing;

namespace Mateus.Controllers
{
    public class UserController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public UserController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IUsersRepository usersRepository = new UsersRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "UserPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<UserAccountView> users = UserAccountView.GetUserAccountView(usersRepository.GetValid())
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                users = users.Where(c => c.Username.Contains(searchString) || c.FirstName.Contains(searchString) || c.LastName.Contains(searchString));
            }

            ViewData["numberOfRecords"] = users.Count();

            users = users.Page(page, pageSize);

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("User?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", users.ToList());
            }

        }

        #endregion

        #region Add new User

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            IRolesRepository rolesRepository = new RolesRepository(db);
            UserAccountView userAccountView = new UserAccountView();

            userAccountView.Roles = new MultiSelectList(rolesRepository.GetActiveRoles(), "RolePK", "Name");

            userAccountView.ChangePassword = true;

            return View(userAccountView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(UserAccountView userAccountView, FormCollection form)
        {
            IUsersRepository usersRepository = new UsersRepository(db);

            if(userAccountView.Username != null)
            {
                if(usersRepository.GetUserByUsername(userAccountView.Username) != null)
                {
                    ModelState.AddModelError("Username", "Korisničko ime već postoji.");
                }
            }

            if(userAccountView.Email != null)
            {
                if(usersRepository.GetUserByEmail(userAccountView.Email) != null)
                {
                    ModelState.AddModelError("Email", "E-mail već postoji.");
                }
            }

            if (ModelState.IsValid)
            {
                string sessionToken = Audit.GenerateNewSessionToken();

                User user = new User();

                userAccountView.RegistrationDate = DateTime.Now;

                PBKDF2 pbkdf2 = new PBKDF2(userAccountView.Password);

                var passwordBytes = pbkdf2.ComputePBKDF2();

                userAccountView.Password = Convert.ToBase64String(passwordBytes);
                userAccountView.Salt = Convert.ToBase64String(pbkdf2.SaltBytes);

                string[] rolesSelectedValues = new string[100];
                if (form["RolePK"] != null) rolesSelectedValues = ((string)form["RolePK"]).Split(',');

                userAccountView.ConvertTo(userAccountView, user);

                usersRepository.Add(user);
                usersRepository.SaveChanges(sessionToken);

                // Delete old roles
                IUsersInRolesRepository usersInRolesRepository = new UsersInRolesRepository(db);
                usersInRolesRepository.Delete(uir => uir.UserFK == user.UserPK);

                // Add New Roles
                foreach (string role in rolesSelectedValues)
                {
                    UserInRole tmpUserInRole = new UserInRole();

                    tmpUserInRole.UserFK = user.UserPK;
                    tmpUserInRole.RoleFK = Convert.ToInt32(role);

                    usersInRolesRepository.Add(tmpUserInRole);
                }

                usersInRolesRepository.SaveChanges(sessionToken);

                TempData["message"] = LayoutHelper.GetMessage("INSERT", user.UserPK);

                return RedirectToAction("Index", "User");
            }
            else
            {
                string[] rolesSelectedValues = new string[100];
                if (form["RolePK"] != null) rolesSelectedValues = ((string)form["RolePK"]).Split(',');

                IRolesRepository rolesRepository = new RolesRepository(db);

                userAccountView.Roles = new MultiSelectList(rolesRepository.GetActiveRoles(), "RolePK", "Name", rolesSelectedValues);

                return View(userAccountView);
            }
        }

        #endregion

        #region Edit User

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? userPK)
        {
            if (userPK != null)
            {
                IRolesRepository rolesRepository = new RolesRepository(db);
                IUsersRepository usersRepository = new UsersRepository(db);

                User user = usersRepository.GetUserByUserID((int)userPK);
                UserAccountView userAccountView = new UserAccountView();

                userAccountView.ConvertFrom(user, userAccountView);

                IUsersInRolesRepository usersInRolesRepository = new UsersInRolesRepository(db);

                var rolesSelectedValues = usersInRolesRepository.GetRolesByUserPK((int)userPK).Select( uir => uir.RoleFK);

                userAccountView.Roles = new MultiSelectList(rolesRepository.GetActiveRoles().ToList(), "RolePK", "Name", rolesSelectedValues);

                return View(userAccountView);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(UserAccountView userAccountView, FormCollection form)
        {
            IUsersRepository usersRepository = new UsersRepository(db);

            if(userAccountView.Username != null)
            {
                User user = usersRepository.GetUserByUsername(userAccountView.Username);

                if(user != null && user.UserPK != userAccountView.UserPK)
                {
                    ModelState.AddModelError("Username", "Korisničko ime već postoji.");
                }
            }

            if(userAccountView.Email != null)
            {
                User user = usersRepository.GetUserByEmail(userAccountView.Email);

                if(user != null && user.UserPK != userAccountView.UserPK)
                {
                    ModelState.AddModelError("Email", "E-mail već postoji.");
                }
            }

            if (ModelState.IsValid)
            {
                string sessionToken = Audit.GenerateNewSessionToken();

                IRolesRepository rolesRepository = new RolesRepository(db);

                User user = usersRepository.GetUserByUserID((int)userAccountView.UserPK);

                userAccountView.RegistrationDate = user.RegistrationDate;
                if (userAccountView.ChangePassword)
                {
                    PBKDF2 pbkdf2 = new PBKDF2(userAccountView.Password);

                    var passwordBytes = pbkdf2.ComputePBKDF2();

                    userAccountView.Password = Convert.ToBase64String(passwordBytes);
                    userAccountView.Salt = Convert.ToBase64String(pbkdf2.SaltBytes);
                }
                else
                {
                    userAccountView.Password = user.Password;
                    userAccountView.Salt = user.Salt;
                }

                userAccountView.ConvertTo(userAccountView, user);

                usersRepository.SaveChanges(sessionToken);

                string[] rolesSelectedValues = new string[100];
                if (form["RolePK"] != null) rolesSelectedValues = ((string)form["RolePK"]).Split(',');

                // Delete old roles
                IUsersInRolesRepository usersInRolesRepository = new UsersInRolesRepository(db);
                usersInRolesRepository.Delete(uir => uir.UserFK == user.UserPK);

                // Add New Roles
                foreach (string role in rolesSelectedValues)
                {
                    UserInRole tmpUserInRole = new UserInRole();

                    tmpUserInRole.UserFK = user.UserPK;
                    tmpUserInRole.RoleFK = Convert.ToInt32(role);

                    usersInRolesRepository.Add(tmpUserInRole);
                }

                usersInRolesRepository.SaveChanges(sessionToken);

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", user.UserPK);

                return RedirectToAction("Index", "User");
            }
            else
            {
                string[] rolesSelectedValues = new string[100];
                if (form["RolePK"] != null) rolesSelectedValues = ((string)form["RolePK"]).Split(',');

                IRolesRepository rolesRepository = new RolesRepository(db);

                userAccountView.Roles = new MultiSelectList(rolesRepository.GetActiveRoles(), "RolePK", "Name", rolesSelectedValues);

                return View(userAccountView);
            }
        }

        #endregion

        #region Delete User

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? userPK)
        {
            IUsersRepository usersRepository = new UsersRepository(db);
            if (userPK != null)
            {
                User user = usersRepository.GetUserByUserID((int)userPK);

                user.Deleted = true;

                usersRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", user.UserPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "delete")]
        public ActionResult Activate(int? userPK)
        {
            IUsersRepository usersRepository = new UsersRepository(db);
            if (userPK != null)
            {
                User user = usersRepository.GetUserByUserID((int)userPK);

                user.Active = true;

                usersRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "delete")]
        public ActionResult Deactivate(int? userPK)
        {
            IUsersRepository usersRepository = new UsersRepository(db);
            if (userPK != null)
            {
                User user = usersRepository.GetUserByUserID((int)userPK);

                user.Active = false;

                usersRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        #endregion
    }
}
