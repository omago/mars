using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.PhysicalEntityModel;
using Mateus.Model.BussinesLogic.Views.PhysicalEntityAuditModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class PhysicalEntityController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public PhysicalEntityController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
            PhysicalEntityView physicalEntityView = new PhysicalEntityView();
            IContractsRepository contractsRepository = new ContractsRepository(db);
            ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "PhysicalEntityPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<PhysicalEntityView> physicalEntities = PhysicalEntityView.GetPhysicalEntityView(physicalEntitiesRepository.GetValid())
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                physicalEntities = physicalEntities.Where(c => c.Firstname.Contains(searchString) || c.Lastname.Contains(searchString) || c.OIB.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["dateOfBirth"]) && String.IsNullOrWhiteSpace(Request.QueryString["dateOfBirthTo"]))
            {
                DateTime dateOfBirth = DateTime.ParseExact(Request.QueryString["dateOfBirth"], "dd.MM.yyyy.", null);
                int dateOfBirthMonth = dateOfBirth.Month;
                int dateOfBirthDay = dateOfBirth.Day;

                physicalEntities = physicalEntities.Where(c => c.DateOfBirth.Value.Month == dateOfBirthMonth && c.DateOfBirth.Value.Day == dateOfBirthDay);
            } 
            else if (!String.IsNullOrWhiteSpace(Request.QueryString["dateOfBirth"]) && !String.IsNullOrWhiteSpace(Request.QueryString["dateOfBirthTo"]))
            {
                DateTime dateOfBirthFrom = DateTime.ParseExact(Request.QueryString["dateOfBirth"], "dd.MM.yyyy.", null);
                int dateOfBirthFromMonth = dateOfBirthFrom.Month;
                int dateOfBirthFromDay = dateOfBirthFrom.Day;

                DateTime dateOfBirthTo = DateTime.ParseExact(Request.QueryString["dateOfBirthTo"], "dd.MM.yyyy.", null);
                int dateOfBirthToMonth = dateOfBirthTo.Month;
                int dateOfBirthToDay = dateOfBirthTo.Day;

                physicalEntities = physicalEntities.Where(c => (c.DateOfBirth.Value.Month >= dateOfBirthFromMonth && c.DateOfBirth.Value.Day >= dateOfBirthFromDay) && 
                                                                (c.DateOfBirth.Value.Month <= dateOfBirthToMonth && c.DateOfBirth.Value.Day <= dateOfBirthToDay));
            }

            physicalEntities = physicalEntities.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = physicalEntitiesRepository.GetValid().Where(c => c.Firstname.Contains(searchString) || c.Lastname.Contains(searchString) || c.OIB.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = physicalEntitiesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("PhysicalEntity?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", physicalEntities.ToList());
            }

        }

        #endregion

        #region Add new PhysicalEntity

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? legalEntityFK)
        {
            TempData["legalEntityFK"] = legalEntityFK;

            PhysicalEntityView physicalEntityView = new PhysicalEntityView();

            physicalEntityView.BindDDLs(physicalEntityView, db);

            return View(physicalEntityView);
        }


        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(PhysicalEntityView physicalEntityView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
                PhysicalEntity physicalEntity = new PhysicalEntity();

                physicalEntityView.ConvertTo(physicalEntityView, physicalEntity);

                physicalEntitiesRepository.Add(physicalEntity);
                physicalEntitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", physicalEntity.PhysicalEntityPK);

                return RedirectToAction("Index", "PhysicalEntity");
            }
            else
            {
                physicalEntityView.BindDDLs(physicalEntityView, db);

                return View(physicalEntityView);
            }
        }

        #endregion

        #region Edit PhysicalEntity

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? physicalEntityPK)
        {
            if (physicalEntityPK != null)
            {
                IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
                PhysicalEntity physicalEntity = physicalEntitiesRepository.GetPhysicalEntityByPK((int)physicalEntityPK);
                PhysicalEntityView physicalEntityView = new PhysicalEntityView();

                physicalEntityView.ConvertFrom(physicalEntity, physicalEntityView);
                physicalEntityView.BindDDLs(physicalEntityView, db);

                return View(physicalEntityView);
            }
            else
            {
                return RedirectToAction("Index", "PhysicalEntity");
            }
        }


        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(PhysicalEntityView physicalEntityView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
                PhysicalEntity physicalEntity = physicalEntitiesRepository.GetPhysicalEntityByPK((int)physicalEntityView.PhysicalEntityPK);
                physicalEntityView.ConvertTo(physicalEntityView, physicalEntity);

                physicalEntitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", physicalEntity.PhysicalEntityPK);

                return RedirectToAction("Index", "PhysicalEntity");
            }
            else
            {
                physicalEntityView.BindDDLs(physicalEntityView, db);

                return View(physicalEntityView);
            }
        }

        #endregion

        #region Owner PhysicalEntity
        [PITAuthorize(Roles = "edit")]
        public ActionResult Owner(int? physicalEntityPK)
        {
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
            if (physicalEntityPK != null)
            {
                PhysicalEntity physicalEntity = physicalEntitiesRepository.GetPhysicalEntityByPK((int)physicalEntityPK);

                physicalEntity.Owner = true;

                physicalEntitiesRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "edit")]
        public ActionResult UnOwner(int? physicalEntityPK)
        {
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
            if (physicalEntityPK != null)
            {
                PhysicalEntity physicalEntity = physicalEntitiesRepository.GetPhysicalEntityByPK((int)physicalEntityPK);

                physicalEntity.Owner = false;

                physicalEntitiesRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        #endregion

        #region LegalRepresentative PhysicalEntity
        [PITAuthorize(Roles = "edit")]
        public ActionResult LegalRepresentative(int? physicalEntityPK)
        {
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
            if (physicalEntityPK != null)
            {
                PhysicalEntity physicalEntity = physicalEntitiesRepository.GetPhysicalEntityByPK((int)physicalEntityPK);

                physicalEntity.LegalRepresentative = true;

                physicalEntitiesRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "edit")]
        public ActionResult UnLegalRepresentative(int? physicalEntityPK)
        {
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
            if (physicalEntityPK != null)
            {
                PhysicalEntity physicalEntity = physicalEntitiesRepository.GetPhysicalEntityByPK((int)physicalEntityPK);

                physicalEntity.LegalRepresentative = false;

                physicalEntitiesRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        #endregion

        #region Delete PhysicalEntity
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? physicalEntityPK)
        {
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
            if (physicalEntityPK != null)
            {
                PhysicalEntity physicalEntity = physicalEntitiesRepository.GetPhysicalEntityByPK((int)physicalEntityPK);

                physicalEntity.Deleted = true;

                physicalEntitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", physicalEntity.PhysicalEntityPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        #endregion

        #region Audit
        public ActionResult Audit(int physicalEntityPK)
        {
            List<PhysicalEntityAuditView> physicalEntities = PhysicalEntityAuditView.GetPhysicalEntityAuditView(db, physicalEntityPK);

            ViewBag.physicalEntitiesHistory = physicalEntities.ToList();
            
            return View("Audit");
        }
        #endregion
    }
}