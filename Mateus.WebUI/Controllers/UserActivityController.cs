using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.UserActivityModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class UserActivityController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public UserActivityController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IUserActivitiesRepository userActivitiesRepository = new UserActivitiesRepository(db);
            IUsersRepository usersRepository = new UsersRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "UserActivityPK";
            string ordering = sortColumn + " " + sortOrder;

            ordering = ordering.Trim();

            int numberOfRecords = 0;
            IQueryable<UserActivityView> userActivities = UserActivityView.GetHomeView(userActivitiesRepository.GetValid(), usersRepository.GetValid())
                                                                          .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                userActivities = userActivities.Where(ua => ua.ActivityDescription.Contains(searchString) || ua.UserFullName.Contains(searchString));
            }

            numberOfRecords = userActivities.Count();
            userActivities = userActivities.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = numberOfRecords;
            }
            else
            {
                ViewData["numberOfRecords"] = numberOfRecords;
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("UserActivity?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", userActivities.ToList());
            }

        }

        #endregion

        #region Delete Activity

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? userActivityPK)
        {
            IUserActivitiesRepository userActivitiesRepository = new UserActivitiesRepository(db);
            if (userActivityPK != null)
            {
                UserActivity activity = userActivitiesRepository.GetUserActivityByPK((int)userActivityPK);

                activity.Deleted = true;

                userActivitiesRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}