using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.FulfilledFactorModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class FulfilledFactorController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public FulfilledFactorController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IFulfilledFactorsRepository fulfilledFactorsRepository = new FulfilledFactorsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "FulfilledFactorPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<FulfilledFactor> fulfilledFactors = fulfilledFactorsRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                fulfilledFactors = fulfilledFactors.Where(c => c.Name.Contains(searchString));
            }

            fulfilledFactors = fulfilledFactors.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = fulfilledFactorsRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = fulfilledFactorsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("FulfilledFactor?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", fulfilledFactors.ToList());
            }

        }

        #endregion

        #region Add new FulfilledFactor

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(FulfilledFactorView fulfilledFactorView)
        {
            if (ModelState.IsValid)
            {
                IFulfilledFactorsRepository fulfilledFactorsRepository = new FulfilledFactorsRepository(db);
                FulfilledFactor fulfilledFactor = new FulfilledFactor();

                fulfilledFactorView.ConvertTo(fulfilledFactorView, fulfilledFactor);

                fulfilledFactorsRepository.Add(fulfilledFactor);
                fulfilledFactorsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", fulfilledFactor.FulfilledFactorPK);

                return RedirectToAction("Index", "FulfilledFactor");
            }
            else
            {
                return View(fulfilledFactorView);
            }
        }

        #endregion

        #region Edit FulfilledFactor

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? fulfilledFactorPK)
        {
            if (fulfilledFactorPK != null)
            {
                IFulfilledFactorsRepository fulfilledFactorsRepository = new FulfilledFactorsRepository(db);
                FulfilledFactor fulfilledFactor = fulfilledFactorsRepository.GetFulfilledFactorByPK((int)fulfilledFactorPK);
                FulfilledFactorView fulfilledFactorView = new FulfilledFactorView();

                fulfilledFactorView.ConvertFrom(fulfilledFactor, fulfilledFactorView);

                return View(fulfilledFactorView);
            }
            else
            {
                return RedirectToAction("Index", "FulfilledFactor");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(FulfilledFactorView fulfilledFactorModel)
        {
            if (ModelState.IsValid)
            {
                IFulfilledFactorsRepository fulfilledFactorsRepository = new FulfilledFactorsRepository(db);
                FulfilledFactor fulfilledFactor = fulfilledFactorsRepository.GetFulfilledFactorByPK((int)fulfilledFactorModel.FulfilledFactorPK);
                fulfilledFactorModel.ConvertTo(fulfilledFactorModel, fulfilledFactor);

                fulfilledFactorsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", fulfilledFactor.FulfilledFactorPK);

                return RedirectToAction("Index", "FulfilledFactor");
            }
            else
            {
                return View(fulfilledFactorModel);
            }
        }

        #endregion

        #region Delete FulfilledFactor
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? fulfilledFactorPK)
        {
            IFulfilledFactorsRepository fulfilledFactorsRepository = new FulfilledFactorsRepository(db);
            if (fulfilledFactorPK != null)
            {
                FulfilledFactor fulfilledFactor = fulfilledFactorsRepository.GetFulfilledFactorByPK((int)fulfilledFactorPK);

                fulfilledFactor.Deleted = true;

                fulfilledFactorsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", fulfilledFactor.FulfilledFactorPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
