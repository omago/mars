using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.LegalEntityBranchModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using Mateus.Model.BussinesLogic.Views.LegalEntityBranchAuditModel;

namespace Mateus.Controllers
{
    public class LegalEntityBranchController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public LegalEntityBranchController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "LegalEntityBranchPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<LegalEntityBranchView> branches = LegalEntityBranchView.GetLegalEntityBranchView(legalEntityBranchesRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities())
                                                        .OrderBy(ordering);
            //legalEntities ddl
            ViewBag.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name", Request.QueryString["legalEntityFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                branches = branches.Where(c => c.Name.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["legalEntityFK"]))
            {
                int legalEntityFK = Convert.ToInt32(Request.QueryString["legalEntityFK"]);
                branches = branches.Where(c => c.LegalEntityFK == legalEntityFK);
            }

            branches = branches.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = legalEntityBranchesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = legalEntityBranchesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("LegalEntityBranch?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", branches.ToList());
            }

        }

        #endregion

        #region Add new Branch

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? legalEntityFK)
        {
            LegalEntityBranchView legalEntityBranchView = new LegalEntityBranchView();

            // set default country to Croatia
            legalEntityBranchView.CountryFK = 81;

            //legalEntities ddl
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);

            if (legalEntityFK != null)
            {
                TempData["legalEntityFK"] = legalEntityFK;
                legalEntityBranchView.LegalEntityFK = (int)legalEntityFK;
                legalEntityBranchView.LegalEntityName = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityFK).Name;
            }

            legalEntityBranchView.BindDDLs(legalEntityBranchView, db);

            return View(legalEntityBranchView);
        }


        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(LegalEntityBranchView legalEntityBranchView, FormCollection form, int? legalEntityFK)
        {
            if (ModelState.IsValid)
            {
                ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
                LegalEntityBranch legalEntityBranch = new LegalEntityBranch();

                legalEntityBranchView.ConvertTo(legalEntityBranchView, legalEntityBranch);

                legalEntityBranchesRepository.Add(legalEntityBranch);
                legalEntityBranchesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", legalEntityBranch.LegalEntityBranchPK);

                if (TempData["legalEntityFK"] != null) return RedirectToAction("Index", "LegalEntity");
                else return RedirectToAction("Index", "LegalEntityBranch");
            }
            else
            {
                legalEntityBranchView.BindDDLs(legalEntityBranchView, db);

                return View(legalEntityBranchView);
            }
        }

        #endregion

        #region Edit Branch

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? legalEntityBranchPK)
        {
            if (legalEntityBranchPK != null)
            {
                ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
                LegalEntityBranch branch = legalEntityBranchesRepository.GetLegalEntityBranchByPK((int)legalEntityBranchPK);
                LegalEntityBranchView legalEntityBranchView = new LegalEntityBranchView();

                legalEntityBranchView.ConvertFrom(branch, legalEntityBranchView, db);
                legalEntityBranchView.BindDDLs(legalEntityBranchView, db);

                return View(legalEntityBranchView);
            }
            else
            {
                return RedirectToAction("Index", "LegalEntityBranch");
            }
        }


        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(LegalEntityBranchView legalEntityBranchView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
                LegalEntityBranch legalEntityBranch = legalEntityBranchesRepository.GetLegalEntityBranchByPK((int)legalEntityBranchView.LegalEntityBranchPK);
                legalEntityBranchView.ConvertTo(legalEntityBranchView, legalEntityBranch);

                legalEntityBranchesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", legalEntityBranch.LegalEntityBranchPK);

                return RedirectToAction("Index", "LegalEntityBranch");
            }
            else
            {
                legalEntityBranchView.BindDDLs(legalEntityBranchView, db);

                return View(legalEntityBranchView);
            }
        }

        #endregion

        #region Delete Branch
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? legalEntityBranchPK)
        {
            ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
            if (legalEntityBranchPK != null)
            {
                LegalEntityBranch legalEntityBranch = legalEntityBranchesRepository.GetLegalEntityBranchByPK((int)legalEntityBranchPK);

                legalEntityBranch.Deleted = true;

                legalEntityBranchesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", legalEntityBranch.LegalEntityBranchPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

        #region Audit
        public ActionResult Audit(int legalEntityBranchPK)
        {
            List<LegalEntityBranchAuditView> legalEntityBranches = LegalEntityBranchAuditView.GetLegalEntityBranchAuditView(db, legalEntityBranchPK);

            ViewBag.LegalEntityBranchesHistory = legalEntityBranches;
            
            return View("Audit");
        }
        #endregion
    }
}
