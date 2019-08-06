using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.AdditionalFactorModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class AdditionalFactorController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public AdditionalFactorController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IAdditionalFactorsRepository additionalFactorsRepository = new AdditionalFactorsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "AdditionalFactorPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<AdditionalFactor> additionalFactors = additionalFactorsRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                additionalFactors = additionalFactors.Where(c => c.Name.Contains(searchString));
            }

            additionalFactors = additionalFactors.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = additionalFactorsRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = additionalFactorsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("AdditionalFactor?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", additionalFactors.ToList());
            }

        }

        #endregion

        #region Add new AdditionalFactor

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(AdditionalFactorView additionalFactorView)
        {
            if (ModelState.IsValid)
            {
                IAdditionalFactorsRepository additionalFactorsRepository = new AdditionalFactorsRepository(db);
                AdditionalFactor additionalFactor = new AdditionalFactor();

                additionalFactorView.ConvertTo(additionalFactorView, additionalFactor);

                additionalFactorsRepository.Add(additionalFactor);
                additionalFactorsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", additionalFactor.AdditionalFactorPK);

                return RedirectToAction("Index", "AdditionalFactor");
            }
            else
            {
                return View(additionalFactorView);
            }
        }

        #endregion

        #region Edit AdditionalFactor

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? additionalFactorPK)
        {
            if (additionalFactorPK != null)
            {
                IAdditionalFactorsRepository additionalFactorsRepository = new AdditionalFactorsRepository(db);
                AdditionalFactor additionalFactor = additionalFactorsRepository.GetAdditionalFactorByPK((int)additionalFactorPK);
                AdditionalFactorView additionalFactorView = new AdditionalFactorView();

                additionalFactorView.ConvertFrom(additionalFactor, additionalFactorView);

                return View(additionalFactorView);
            }
            else
            {
                return RedirectToAction("Index", "AdditionalFactor");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(AdditionalFactorView additionalFactorView)
        {
            if (ModelState.IsValid)
            {
                IAdditionalFactorsRepository additionalFactorsRepository = new AdditionalFactorsRepository(db);
                AdditionalFactor additionalFactor = additionalFactorsRepository.GetAdditionalFactorByPK((int)additionalFactorView.AdditionalFactorPK);
                additionalFactorView.ConvertTo(additionalFactorView, additionalFactor);

                additionalFactorsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", additionalFactor.AdditionalFactorPK);

                return RedirectToAction("Index", "AdditionalFactor");
            }
            else
            {
                return View(additionalFactorView);
            }
        }

        #endregion

        #region Delete AdditionalFactor
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? additionalFactorPK)
        {
            IAdditionalFactorsRepository additionalFactorsRepository = new AdditionalFactorsRepository(db);
            if (additionalFactorPK != null)
            {
                AdditionalFactor additionalFactor = additionalFactorsRepository.GetAdditionalFactorByPK((int)additionalFactorPK);

                additionalFactor.Deleted = true;

                TempData["message"] = LayoutHelper.GetMessage("DELETE", additionalFactor.AdditionalFactorPK);

                additionalFactorsRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
