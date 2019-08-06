using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.LegalEntityOwnerModel;
using System.Web.Routing;
using Mateus.Support;
using PITFramework.Support.Grid;
using System.Data.Objects.SqlClient;
using Mateus.Model.BussinesLogic.Support.DDL;
using Mateus.Model.BussinesLogic.Views.LegalEntityOwnerAuditModel;

namespace Mateus.Controllers
{
    public class LegalEntityOwnerController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public LegalEntityOwnerController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);  

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "LegalEntityOwnerPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            List<LegalEntityOwner> filteredCoList = new List<LegalEntityOwner>();
            IQueryable<LegalEntityOwner> legalEntityOwnersTable = legalEntityOwnersRepository.GetValid();

            //if (!String.IsNullOrWhiteSpace(Request.QueryString["legalEntityFK"]))
            //{
            //    List<LegalEntityOwner> coList = new List<LegalEntityOwner>();
            //    coList = legalEntityOwnersRepository.GetFirstLegalEntityOwnersForLegalEntity(Convert.ToInt32(Request.QueryString["legalEntityFK"])).ToList();

            //    foreach (var co in coList)
            //    {
            //        LegalEntityOwnerView.Fill(filteredCoList, legalEntityOwnersTable.ToList(), co);
            //    }

            //    legalEntityOwnersTable = filteredCoList.AsQueryable();
            //}

            IQueryable<LegalEntityOwnerView> legalEntityOwners = LegalEntityOwnerView.GetLegalEntityOwnerView(legalEntityOwnersTable, 
                                                                                               physicalEntitiesRepository.GetValid(), 
                                                                                               legalEntitiesRepository.GetValid())
                                                                         .OrderBy(ordering);
             
            //LegalEntities ddl
            ViewBag.LegalEntities = new SelectList(legalEntitiesRepository.GetValid().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name", Request.QueryString["legalEntityFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["legalEntityFK"]))
            {
                int legalEntityFK = Convert.ToInt32(Request.QueryString["legalEntityFK"]);
                legalEntityOwners = legalEntityOwners.Where(c => c.LegalEntityFK == legalEntityFK);
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                legalEntityOwners = legalEntityOwners.Where(c => c.OwnerName.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = legalEntityOwners.Where(c => c.OwnerName.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = legalEntityOwners.Count();
            }

            legalEntityOwners = legalEntityOwners.Page(page, pageSize);

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("LegalEntityOwner?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", legalEntityOwners.ToList());
            }

        }

        #endregion

        #region Add new LegalEntityOwner

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? legalEntityFK)
        {
            LegalEntityOwnerView legalEntityOwnerView = new LegalEntityOwnerView();

            //LegalEntities ddl
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            if (legalEntityFK != null)
            {
                TempData["legalEntityFK"] = legalEntityFK;
                legalEntityOwnerView.LegalEntityFK = (int)legalEntityFK;
                legalEntityOwnerView.LegalEntityName = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityFK).Name;
            }

            legalEntityOwnerView.BindDLLs(legalEntityOwnerView, db);

            return View(legalEntityOwnerView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(LegalEntityOwnerView legalEntityOwnerView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
                LegalEntityOwner legalEntityOwner = new LegalEntityOwner();

                legalEntityOwnerView.ConvertTo(legalEntityOwnerView, legalEntityOwner);

                legalEntityOwnersRepository.Add(legalEntityOwner);
                legalEntityOwnersRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", legalEntityOwner.LegalEntityOwnerPK);

                if (TempData["legalEntityFK"] != null) return RedirectToAction("Index", "LegalEntity");
                else return RedirectToAction("Index", "LegalEntityOwner");
            }
            else
            {
                legalEntityOwnerView.BindDLLs(legalEntityOwnerView, db);

                return View(legalEntityOwnerView);
            }
        }

        #endregion

        #region Edit LegalEntityOwner

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? legalEntityOwnerPK)
        {
            if (legalEntityOwnerPK != null)
            {
                ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
                LegalEntityOwner legalEntityOwner = legalEntityOwnersRepository.GetLegalEntityOwnerByPK((int)legalEntityOwnerPK);
                LegalEntityOwnerView legalEntityOwnerView = new LegalEntityOwnerView();

                legalEntityOwnerView.ConvertFrom(legalEntityOwner, legalEntityOwnerView, db);
                legalEntityOwnerView.BindDLLs(legalEntityOwnerView, db);
                
                return View(legalEntityOwnerView);
            }
            else
            {
                return RedirectToAction("Index", "LegalEntityOwner");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(LegalEntityOwnerView legalEntityOwnerView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
                LegalEntityOwner legalEntityOwner = legalEntityOwnersRepository.GetLegalEntityOwnerByPK((int)legalEntityOwnerView.LegalEntityOwnerPK);
                legalEntityOwnerView.ConvertTo(legalEntityOwnerView, legalEntityOwner);

                legalEntityOwnersRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", legalEntityOwner.LegalEntityOwnerPK);

                return RedirectToAction("Index", "LegalEntityOwner");
            }
            else
            {
                legalEntityOwnerView.BindDLLs(legalEntityOwnerView, db);

                return View(legalEntityOwnerView);
            }
        }
        #endregion

        #region Delete LegalEntityOwner
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? legalEntityOwnerPK)
        {
            ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
            if (legalEntityOwnerPK != null)
            {
                LegalEntityOwner legalEntityOwner = legalEntityOwnersRepository.GetLegalEntityOwnerByPK((int)legalEntityOwnerPK);

                legalEntityOwner.Deleted = true;

                legalEntityOwnersRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", legalEntityOwner.LegalEntityOwnerPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        #endregion

        #region Audit
        public ActionResult Audit(int legalEntityOwnerPK)
        {
            List<LegalEntityOwnerAuditView> legalEntityOwners = LegalEntityOwnerAuditView.GetLegalEntityOwnerAuditView(db, legalEntityOwnerPK);

            ViewBag.legalEntityOwnersHistory = legalEntityOwners;
            
            return View("Audit");
        }
        #endregion
    }
}