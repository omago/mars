using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.LegalEntityBankModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using Mateus.Model.BussinesLogic.Views.LegalEntityBankAuditModel;

namespace Mateus.Controllers
{
    public class LegalEntityBankController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public LegalEntityBankController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);
            IBanksRepository banksRepository = new BanksRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            LegalEntityBankView legalEntityBankView = new LegalEntityBankView();

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "LegalEntityBankPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<LegalEntityBankView> legalEntitiesBanks = LegalEntityBankView.GetLegalEntityBankView(legalEntitiesBanksRepository.GetValid(), banksRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities())
                                                    .OrderBy(ordering);

            //legalEntities ddl
            ViewBag.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name", Request.QueryString["legalEntityFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                legalEntitiesBanks = legalEntitiesBanks.Where(c => c.LegalEntityName.Contains(searchString) || c.BankName.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["legalEntityFK"]))
            {
                int legalEntityFK = Convert.ToInt32(Request.QueryString["legalEntityFK"]);
                legalEntitiesBanks = legalEntitiesBanks.Where(c => c.LegalEntityFK == legalEntityFK);
            }

            legalEntitiesBanks = legalEntitiesBanks.Page(page, pageSize);
                                                    
            ViewData["numberOfRecords"] = legalEntitiesBanksRepository.GetValid().Count();

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if(page > numberOfPages) {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("LegalEntityBank?" + url + "page=" + numberOfPages);
            } else {
                return View("Index", legalEntitiesBanks.ToList());
            }

        }

        #endregion

        #region Add new LegalEntityBank

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? legalEntityFK)
        {
            LegalEntityBankView legalEntityBankView = new LegalEntityBankView();

            //legalEntities ddl
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            if (legalEntityFK != null)
            {
                TempData["legalEntityFK"] = legalEntityFK;
                legalEntityBankView.LegalEntityFK = (int)legalEntityFK;
                legalEntityBankView.LegalEntityName = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityFK).Name;
            }

            legalEntityBankView.BindDLLs(legalEntityBankView, db);

            return View(legalEntityBankView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(LegalEntityBankView legalEntityBankView)
        {
            if (ModelState.IsValid)
            {
                ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);
                LegalEntityBank legalEntityBank = new LegalEntityBank();

                legalEntityBankView.ConvertTo(legalEntityBankView, legalEntityBank);

                legalEntitiesBanksRepository.Add(legalEntityBank);
                legalEntitiesBanksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", legalEntityBank.LegalEntityBankPK);

                if (TempData["legalEntityFK"] != null) return RedirectToAction("Index", "LegalEntity");
                else return RedirectToAction("Index", "LegalEntityBank");
            }
            else
            {
                legalEntityBankView.BindDLLs(legalEntityBankView, db);

                return View(legalEntityBankView);
            }
        }

        #endregion

        #region Edit LegalEntityBank

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? legalEntityBankPK)
        {
            if (legalEntityBankPK != null)
            {
                ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);
                LegalEntityBank legalEntityBank = legalEntitiesBanksRepository.GetLegalEntityBankByPK((int)legalEntityBankPK);
                LegalEntityBankView legalEntityBankView = new LegalEntityBankView();

                legalEntityBankView.ConvertFrom(legalEntityBank, legalEntityBankView, db);
                legalEntityBankView.BindDLLs(legalEntityBankView, db);

                return View(legalEntityBankView);
            }
            else 
            {
                return RedirectToAction("Index", "LegalEntityBank");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(LegalEntityBankView legalEntityBankView)
        {
            if (ModelState.IsValid)
            {
                ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);
                LegalEntityBank legalEntityBank = legalEntitiesBanksRepository.GetLegalEntityBankByPK((int)legalEntityBankView.LegalEntityBankPK);
                legalEntityBankView.ConvertTo(legalEntityBankView, legalEntityBank);

                legalEntitiesBanksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", legalEntityBank.LegalEntityBankPK);

                return RedirectToAction("Index", "LegalEntityBank");
            }
            else 
            {
                legalEntityBankView.BindDLLs(legalEntityBankView, db);

                return View(legalEntityBankView);
            }
        }

        #endregion

        #region Delete LegalEntityBank
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? legalEntityBankPK)
        {
            ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);
            if (legalEntityBankPK != null)
            {
                LegalEntityBank legalEntityBank = legalEntitiesBanksRepository.GetLegalEntityBankByPK((int)legalEntityBankPK);

                legalEntityBank.Deleted = true;

                legalEntitiesBanksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", legalEntityBank.LegalEntityBankPK);
            }
               
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

        #region Audit
        public ActionResult Audit(int legalEntityBankPK)
        {
            List<LegalEntityBankAuditView> legalEntityBanks = LegalEntityBankAuditView.GetLegalEntityBankAuditView(db, legalEntityBankPK);

            List<DateTime> legalEntityBanksDates = new List<DateTime>();

            foreach(LegalEntityBankAuditView legalEntityBankAuditView in legalEntityBanks)
            {
                if(!legalEntityBanksDates.Contains(legalEntityBankAuditView.ChangeDate.Value))
                {
                    legalEntityBanksDates.Add(legalEntityBankAuditView.ChangeDate.Value);
                }
            }

            ViewBag.legalEntityBanksDatesHistory = legalEntityBanksDates.OrderBy(c => c.Date).ToList();
            ViewBag.legalEntityBanksHistory = legalEntityBanks;
            
            return View("Audit");
        }
        #endregion
    }
}
