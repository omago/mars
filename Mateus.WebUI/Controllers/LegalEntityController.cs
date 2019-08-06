using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.LegalEntityModel;
using System.Web.Routing;
using Mateus.Support;
using PITFramework.Support.Grid;
using System.Data.Objects.SqlClient;
using Mateus.Model.BussinesLogic.Views.LegalEntityAuditModel;

namespace Mateus.Controllers
{
    public class LegalEntityController : Controller
    {
        #region Initalizers

        List<int> legalEntitiesPKToExclude;
        Mateus_wcEntities db = null;
        public LegalEntityController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        //[OutputCache(Order = 1, Duration = 300, VaryByParam = "*", VaryByContentEncoding = "gzip; deflate")]
        //[EnableCompression(Order = 0)]
        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IContractsRepository contractsRepository = new ContractsRepository(db);
            ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
            IBanksRepository banksRepository = new BanksRepository(db);
            ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);
            ILegalEntityLegalRepresentativesRepository legalEntityLegalRepresentativesRepository = new LegalEntityLegalRepresentativesRepository(db);
            ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
            IAssessmentsRepository assessmentsRepository = new AssessmentsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "LegalEntityPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<LegalEntityView> legalEntities = LegalEntityView.GetLegalEntityView(legalEntitiesRepository.GetValid(),
                                                                            legalEntityBranchesRepository.GetValid(),
                                                                            contractsRepository.GetValid(),
                                                                            banksRepository.GetValid(),
                                                                            legalEntitiesBanksRepository.GetValid(),
                                                                            legalEntityLegalRepresentativesRepository.GetValid(),
                                                                            legalEntityOwnersRepository.GetValid(),
                                                                            assessmentsRepository.GetValid())
                                                            .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"];
                legalEntities = legalEntities.Where(c => c.Name.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["owner"]) && Request.QueryString["owner"].Contains("true"))
            {
                legalEntities = legalEntities.Where(c => c.Owner == true);
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["company"]) && Request.QueryString["company"].Contains("true"))
            {
                legalEntities = legalEntities.Where(c => c.Company == true);
            }

            ViewData["numberOfRecords"] = legalEntities.Count();

            legalEntities = legalEntities.Page(page, pageSize);

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("LegalEntity?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", legalEntities.ToList());
            }
        }

        #endregion

        #region Add new LegalEntity

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            LegalEntityView legalEntityView = new LegalEntityView();

            legalEntityView.Active = true;
            legalEntityView.BindDDLs(legalEntityView, db);

            return View(legalEntityView);
        }


        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(LegalEntityView legalEntityView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                LegalEntity legalEntity = new LegalEntity();

                legalEntityView.ConvertTo(legalEntityView, legalEntity);

                legalEntitiesRepository.Add(legalEntity);
                legalEntitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", legalEntity.LegalEntityPK);

                return RedirectToAction("Index", "LegalEntity");
            }
            else
            {
                legalEntityView.BindDDLs(legalEntityView, db);

                return View(legalEntityView);
            }
        }

        #endregion

        #region Edit LegalEntity

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? legalEntityPK)
        {
            if (legalEntityPK != null)
            {
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityPK);
                LegalEntityView legalEntityView = new LegalEntityView();

                legalEntityView.ConvertFrom(legalEntity, legalEntityView);
                legalEntityView.BindDDLs(legalEntityView, db);

                return View(legalEntityView);
            }
            else
            {
                return RedirectToAction("Index", "LegalEntity");
            }
        }


        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(LegalEntityView legalEntityView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);

                LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityView.LegalEntityPK);
                legalEntityView.ConvertTo(legalEntityView, legalEntity);

                legalEntitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", legalEntity.LegalEntityPK);

                return RedirectToAction("Index", "LegalEntity");
            }
            else
            {
                legalEntityView.BindDDLs(legalEntityView, db);

                return View(legalEntityView);
            }
        }

        #endregion

        #region Owner LegalEntity
        [PITAuthorize(Roles = "edit")]
        public ActionResult Owner(int? legalEntityPK)
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            if (legalEntityPK != null)
            {
                LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityPK);

                legalEntity.Owner = true;

                legalEntitiesRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "edit")]
        public ActionResult UnOwner(int? legalEntityPK)
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            if (legalEntityPK != null)
            {
                LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityPK);

                legalEntity.Owner = false;

                legalEntitiesRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        #endregion

        #region Delete LegalEntity
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? legalEntityPK)
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            if (legalEntityPK != null)
            {
                LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityPK);

                legalEntity.Deleted = true;

                legalEntitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", legalEntity.LegalEntityPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "delete")]
        public ActionResult DeleteTemporary(int? legalEntityPK)
        {
            if (legalEntityPK != null)
            {
                legalEntitiesPKToExclude = new List<int>();

                legalEntitiesPKToExclude.Add((int)legalEntityPK);
                Session["legalEntitiesPKToExclude"] = legalEntitiesPKToExclude;
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

        #region Audit LegalEntity

        public ActionResult Audit(int legalEntityPK)
        {
            List<LegalEntityAuditView> legalEntityAuditView = LegalEntityAuditView.GetLegalEntityAuditView(db, legalEntityPK);

            ViewBag.legalEntityHistory = legalEntityAuditView.ToList();
            
            return View("Audit");
        }

        #endregion
    }
}
