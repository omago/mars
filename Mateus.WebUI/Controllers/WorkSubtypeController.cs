using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.WorkSubtypeModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class WorkSubtypeController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public WorkSubtypeController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
            IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "WorkSubtypePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<WorkSubtypeView> workSubtypes = WorkSubtypeView.GetWorkSubtypeView(workSubtypesRepository.GetValid(), workTypesRepository.GetValid())  
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            { 
                string searchString = Request.QueryString["searchString"].ToString();
                workSubtypes = workSubtypes.Where(c => c.Name.Contains(searchString));
            }

            workSubtypes = workSubtypes.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"])) {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = workSubtypesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            } else {
                ViewData["numberOfRecords"] = workSubtypesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if(page > numberOfPages) {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("WorkSubtype?" + url + "page=" + numberOfPages);
            } else {
                return View("Index", workSubtypes.ToList());
            }
            
        }

        #endregion

        #region Add new WorkSubtype

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            WorkSubtypeView workSubtypeView = new WorkSubtypeView();

            workSubtypeView.BindDDLs(workSubtypeView, db);

            return View(workSubtypeView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(WorkSubtypeView workSubtypeView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
                WorkSubtype workSubtype = new WorkSubtype();

                workSubtypeView.ConvertTo(workSubtypeView, workSubtype);

                workSubtypesRepository.Add(workSubtype);
                workSubtypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", workSubtype.WorkSubtypePK);

                return RedirectToAction("Index", "WorkSubtype");
            }
            else
            {
                workSubtypeView.BindDDLs(workSubtypeView, db);

                return View(workSubtypeView);
            }
        }

        #endregion

        #region Edit WorkSubtype

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? workSubtypePK)
        {
            if (workSubtypePK != null)
            {
                IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
                WorkSubtype workSubtype = workSubtypesRepository.GetWorkSubtypeByPK((int)workSubtypePK);
                WorkSubtypeView workSubtypeView = new WorkSubtypeView();

                workSubtypeView.ConvertFrom(workSubtype, workSubtypeView);
                workSubtypeView.BindDDLs(workSubtypeView, db);

                return View(workSubtypeView);
            }
            else
            {
                return RedirectToAction("Index", "WorkSubtype");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(WorkSubtypeView workSubtypeView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
                WorkSubtype workSubtype = workSubtypesRepository.GetWorkSubtypeByPK((int)workSubtypeView.WorkSubtypePK);

                workSubtypeView.ConvertTo(workSubtypeView, workSubtype);

                workSubtypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", workSubtype.WorkSubtypePK);

                return RedirectToAction("Index", "WorkSubtype");
            }
            else
            {
                workSubtypeView.BindDDLs(workSubtypeView, db);

                return View(workSubtypeView);
            }
        }

        #endregion

        #region Delete WorkSubtype

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? workSubtypePK)
        {
            IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
            if (workSubtypePK != null)
            {
                WorkSubtype workSubtype = workSubtypesRepository.GetWorkSubtypeByPK((int)workSubtypePK);

                workSubtype.Deleted = true;

                workSubtypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", workSubtype.WorkSubtypePK);
            }
           
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
