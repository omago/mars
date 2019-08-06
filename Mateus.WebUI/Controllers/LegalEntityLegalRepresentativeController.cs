using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.LegalEntityLegalRepresentativeModel;
using Mateus.Model.BussinesLogic.Views.PhysicalEntityModel;
using System.Web.Routing;
using Mateus.Support;
using PITFramework.Support.Grid;
using System.Data.Objects.SqlClient;
using Mateus.Model.BussinesLogic.Views.LegalEntityLegalRepresentativeAuditModel;

namespace Mateus.Controllers
{
    public class LegalEntityLegalRepresentativeController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public LegalEntityLegalRepresentativeController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ILegalEntityLegalRepresentativesRepository legalEntityLegalRepresentativesRepository = new LegalEntityLegalRepresentativesRepository(db);
            LegalEntityLegalRepresentativeView legalEntityLegalRepresentativeView = new LegalEntityLegalRepresentativeView();
            IContractsRepository contractsRepository = new ContractsRepository(db);
            ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "LegalEntityLegalRepresentativePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<LegalEntityLegalRepresentativeView> legalEntityLegalRepresentatives = LegalEntityLegalRepresentativeView.GetLegalEntityLegalRepresentativeView(legalEntityLegalRepresentativesRepository.GetValid(), legalEntitiesRepository.GetValid(), physicalEntitiesRepository.GetValid())
                                                        .OrderBy(ordering);

            //legalEntities ddl
            ViewBag.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name", Request.QueryString["legalEntityFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["legalEntityFK"]))
            {
                int legalEntityFK = Convert.ToInt32(Request.QueryString["legalEntityFK"]);
                legalEntityLegalRepresentatives = legalEntityLegalRepresentatives.Where(c => c.LegalEntityFK == legalEntityFK);
            }

            legalEntityLegalRepresentatives = legalEntityLegalRepresentatives.Page(page, pageSize);

            ViewData["numberOfRecords"] = legalEntityLegalRepresentatives.Count();

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("LegalEntityLegalRepresentative?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", legalEntityLegalRepresentatives.ToList());
            }

        }

        #endregion

        #region Add new LegalEntityLegalRepresentative

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? legalEntityFK)
        {
            LegalEntityLegalRepresentativeView legalEntityLegalRepresentativeView = new LegalEntityLegalRepresentativeView();

            if (legalEntityFK != null)
            {
                TempData["legalEntityFK"] = legalEntityFK;

                legalEntityLegalRepresentativeView.LegalEntityFK = (int)legalEntityFK;

                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                legalEntityLegalRepresentativeView.LegalEntityName = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityFK).Name;
            }

            legalEntityLegalRepresentativeView.BindDDLs(legalEntityLegalRepresentativeView, db);

            return View(legalEntityLegalRepresentativeView);
        }


        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(LegalEntityLegalRepresentativeView legalEntityLegalRepresentativeView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ILegalEntityLegalRepresentativesRepository legalEntityLegalRepresentativesRepository = new LegalEntityLegalRepresentativesRepository(db);
                LegalEntityLegalRepresentative legalEntityLegalRepresentative = new LegalEntityLegalRepresentative();

                legalEntityLegalRepresentativeView.ConvertTo(legalEntityLegalRepresentativeView, legalEntityLegalRepresentative);

                legalEntityLegalRepresentativesRepository.Add(legalEntityLegalRepresentative);
                legalEntityLegalRepresentativesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", legalEntityLegalRepresentative.LegalEntityLegalRepresentativePK);

                if (TempData["legalEntityFK"] != null) return RedirectToAction("Index", "LegalEntity");
                else return RedirectToAction("Index", "LegalEntityLegalRepresentative");
            }
            else
            {
                legalEntityLegalRepresentativeView.BindDDLs(legalEntityLegalRepresentativeView, db);

                return View(legalEntityLegalRepresentativeView);
            }
        }

        #endregion

        #region Edit LegalEntityLegalRepresentative

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? legalEntityLegalRepresentativePK)
        {
            if (legalEntityLegalRepresentativePK != null)
            {
                ILegalEntityLegalRepresentativesRepository legalEntityLegalRepresentativesRepository = new LegalEntityLegalRepresentativesRepository(db);
                LegalEntityLegalRepresentative legalEntityLegalRepresentative = legalEntityLegalRepresentativesRepository.GetLegalEntityLegalRepresentativeByPK((int)legalEntityLegalRepresentativePK);
                LegalEntityLegalRepresentativeView legalEntityLegalRepresentativeView = new LegalEntityLegalRepresentativeView();

                legalEntityLegalRepresentativeView.ConvertFrom(legalEntityLegalRepresentative, legalEntityLegalRepresentativeView, db);
                legalEntityLegalRepresentativeView.BindDDLs(legalEntityLegalRepresentativeView, db);

                return View(legalEntityLegalRepresentativeView);
            }
            else
            {
                return RedirectToAction("Index", "LegalEntityLegalRepresentative");
            }
        }


        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(LegalEntityLegalRepresentativeView legalEntityLegalRepresentativeView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ILegalEntityLegalRepresentativesRepository legalEntityLegalRepresentativesRepository = new LegalEntityLegalRepresentativesRepository(db);
                LegalEntityLegalRepresentative legalEntityLegalRepresentative = legalEntityLegalRepresentativesRepository.GetLegalEntityLegalRepresentativeByPK((int)legalEntityLegalRepresentativeView.LegalEntityLegalRepresentativePK);
                legalEntityLegalRepresentativeView.ConvertTo(legalEntityLegalRepresentativeView, legalEntityLegalRepresentative);

                legalEntityLegalRepresentativesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", legalEntityLegalRepresentative.LegalEntityLegalRepresentativePK);

                return RedirectToAction("Index", "LegalEntityLegalRepresentative");
            }
            else
            {
                legalEntityLegalRepresentativeView.BindDDLs(legalEntityLegalRepresentativeView, db);

                return View(legalEntityLegalRepresentativeView);
            }
        }

        #endregion

        #region Delete LegalEntityLegalRepresentative
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? legalEntityLegalRepresentativePK)
        {
            ILegalEntityLegalRepresentativesRepository legalEntityLegalRepresentativesRepository = new LegalEntityLegalRepresentativesRepository(db);
            if (legalEntityLegalRepresentativePK != null)
            {
                LegalEntityLegalRepresentative legalEntityLegalRepresentative = legalEntityLegalRepresentativesRepository.GetLegalEntityLegalRepresentativeByPK((int)legalEntityLegalRepresentativePK);

                legalEntityLegalRepresentative.Deleted = true;

                legalEntityLegalRepresentativesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", legalEntityLegalRepresentative.LegalEntityLegalRepresentativePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

        #region Audit
        public ActionResult Audit(int legalEntityLegalRepresentativePK)
        {
            List<LegalEntityLegalRepresentativeAuditView> legalEntityLegalRepresentatives = LegalEntityLegalRepresentativeAuditView.GetLegalEntityLegalRepresentativeAuditView(db, legalEntityLegalRepresentativePK);

            ViewBag.legalEntityLegalRepresentativesHistory = legalEntityLegalRepresentatives;
            
            return View("Audit");
        }
        #endregion
    }
}
