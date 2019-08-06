using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.WorkTypeModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class WorkTypeController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public WorkTypeController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "WorkTypePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<WorkType> workTypes = workTypesRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                workTypes = workTypes.Where(c => c.Name.Contains(searchString));
            }

            workTypes = workTypes.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = workTypesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = workTypesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("WorkType?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", workTypes.ToList());
            }

        }

        #endregion

        #region Add new WorkType

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(WorkTypeView workTypeView)
        {
            if (ModelState.IsValid)
            {
                IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
                WorkType workType = new WorkType();

                workTypeView.ConvertTo(workTypeView, workType);

                workTypesRepository.Add(workType);
                workTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", workType.WorkTypePK);

                return RedirectToAction("Index", "WorkType");
            }
            else
            {
                return View(workTypeView);
            }
        }

        #endregion

        #region Edit WorkType

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? workTypePK)
        {
            if (workTypePK != null)
            {
                IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
                WorkType workType = workTypesRepository.GetWorkTypeByPK((int)workTypePK);
                WorkTypeView workTypeView = new WorkTypeView();

                workTypeView.ConvertFrom(workType, workTypeView);

                return View(workTypeView);
            }
            else
            {
                return RedirectToAction("Index", "WorkType");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(WorkTypeView workTypeModel)
        {
            if (ModelState.IsValid)
            {
                IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
                WorkType workType = workTypesRepository.GetWorkTypeByPK((int)workTypeModel.WorkTypePK);
                workTypeModel.ConvertTo(workTypeModel, workType);

                workTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", workType.WorkTypePK);

                return RedirectToAction("Index", "WorkType");
            }
            else
            {
                return View(workTypeModel);
            }
        }

        #endregion

        #region Delete WorkType
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? workTypePK)
        {
            IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
            if (workTypePK != null)
            {
                WorkType workType = workTypesRepository.GetWorkTypeByPK((int)workTypePK);

                workType.Deleted = true;

                workTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", workType.WorkTypePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
