using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.SubstationModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class SubstationController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public SubstationController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ISubstationsRepository substationsRepository = new SubstationsRepository(db);
            IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "SubstationPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<SubstationView> substations = SubstationView.GetSubstationView(substationsRepository.GetValid(), regionalOfficesRepository.GetValid())  
                                                                   .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                substations = substations.Where(c => c.Name.Contains(searchString));
            }

            substations = substations.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = substationsRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = substationsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Substation?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", substations.ToList());
            }
        }

        #endregion

        #region Add new Substation

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            SubstationView substationView = new SubstationView();
            substationView.BindDDLs(substationView, db);

            return View(substationView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(SubstationView substationView)
        {
            if (ModelState.IsValid)
            {
                ISubstationsRepository substationsRepository = new SubstationsRepository(db);
                Substation substation = new Substation();

                substationView.ConvertTo(substationView, substation);

                substationsRepository.Add(substation);
                substationsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", substation.SubstationPK);

                return RedirectToAction("Index", "Substation");
            }
            else
            {
                substationView.BindDDLs(substationView, db);

                return View(substationView);
            }
        }

        #endregion

        #region Edit Substation

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? substationPK)
        {
            if (substationPK != null)
            {
                ISubstationsRepository substationsRepository = new SubstationsRepository(db);
                Substation substation = substationsRepository.GetSubstationByPK((int)substationPK);
                SubstationView substationView = new SubstationView();

                substationView.ConvertFrom(substation, substationView);
                substationView.BindDDLs(substationView, db);

                return View(substationView);
            }
            else
            {
                return RedirectToAction("Index", "Substation");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(SubstationView substationView)
        {
            if (ModelState.IsValid)
            {
                ISubstationsRepository substationsRepository = new SubstationsRepository(db);
                Substation substation = substationsRepository.GetSubstationByPK((int)substationView.SubstationPK);
                substationView.ConvertTo(substationView, substation);

                substationsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", substation.SubstationPK);

                return RedirectToAction("Index", "Substation");
            }
            else
            {
                substationView.BindDDLs(substationView, db);

                return View(substationView);
            }
        }

        #endregion

        #region Delete Substation
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? substationPK)
        {
            ISubstationsRepository substationsRepository = new SubstationsRepository(db);
            if (substationPK != null)
            {
                Substation substation = substationsRepository.GetSubstationByPK((int)substationPK);

                substation.Deleted = true;

                substationsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", substation.SubstationPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

    }
}
