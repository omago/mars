using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.ActivityModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class ActivityController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public ActivityController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "ActivityPK";
            string ordering = sortColumn + " " + sortOrder;

            ordering = ordering.Trim();

            IQueryable<ActivityView> activities = ActivityView.GetHomeView(activitiesRepository.GetValid())
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                activities = activities.Where(c => c.Name.Contains(searchString) || c.Code.Contains(searchString));
            }

            activities = activities.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = activitiesRepository.GetValid().Where(c => c.Name.Contains(searchString) || c.Code.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = activitiesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Activity?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", activities.ToList());
            }

        }

        #endregion

        #region Add new Activity

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(ActivityView activityView)
        {
            if (ModelState.IsValid)
            {
                IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);
                Activity activity = new Activity();

                activityView.ConvertTo(activityView, activity);

                activitiesRepository.Add(activity);
                activitiesRepository.SaveChanges();
                
                TempData["message"] = LayoutHelper.GetMessage("INSERT", activity.ActivityPK);

                return RedirectToAction("Index", "Activity");
            }
            else
            {
                return View(activityView);
            }
        }

        #endregion

        #region Edit Activity

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? activityPK)
        {
            if (activityPK != null)
            {
                IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);
                Activity activity = activitiesRepository.GetActivityByPK((int)activityPK);
                ActivityView activityView = new ActivityView();

                activityView.ConvertFrom(activity, activityView);

                return View(activityView);
            }
            else
            {
                return RedirectToAction("Index", "Activity");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(ActivityView activityView)
        {
            if (ModelState.IsValid)
            {
                IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);
                Activity activity = activitiesRepository.GetActivityByPK((int)activityView.ActivityPK);
                activityView.ConvertTo(activityView, activity);

                activitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", activity.ActivityPK);

                return RedirectToAction("Index", "Activity");
            }
            else
            {
                return View(activityView);
            }
        }

        #endregion

        #region Delete Activity

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? activityPK)
        {
            IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);
            if (activityPK != null)
            {
                Activity activity = activitiesRepository.GetActivityByPK((int)activityPK);

                activity.Deleted = true;

                activitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", activity.ActivityPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}