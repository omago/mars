using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.TaxModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class TaxController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public TaxController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ITaxesRepository taxesRepository = new TaxesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "TaxPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<Tax> taxes = taxesRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                taxes = taxes.Where(c => c.Name.Contains(searchString));
            }

            taxes = taxes.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = taxesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = taxesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Tax?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", taxes.ToList());
            }

        }

        #endregion

        #region Add new Tax

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(TaxView taxView)
        {
            if (ModelState.IsValid)
            {
                ITaxesRepository taxesRepository = new TaxesRepository(db);
                Tax tax = new Tax();

                taxView.ConvertTo(taxView, tax);

                taxesRepository.Add(tax);
                taxesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", tax.TaxPK);

                return RedirectToAction("Index", "Tax");
            }
            else
            {
                return View(taxView);
            }
        }

        #endregion

        #region Edit Tax

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? taxPK)
        {
            if (taxPK != null)
            {
                ITaxesRepository taxesRepository = new TaxesRepository(db);
                Tax tax = taxesRepository.GetTaxByPK((int)taxPK);
                TaxView taxView = new TaxView();

                taxView.ConvertFrom(tax, taxView);

                return View(taxView);
            }
            else
            {
                return RedirectToAction("Index", "Tax");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(TaxView taxModel)
        {
            if (ModelState.IsValid)
            {
                ITaxesRepository taxesRepository = new TaxesRepository(db);
                Tax tax = taxesRepository.GetTaxByPK((int)taxModel.TaxPK);
                taxModel.ConvertTo(taxModel, tax);

                taxesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", tax.TaxPK);

                return RedirectToAction("Index", "Tax");
            }
            else
            {
                return View(taxModel);
            }
        }

        #endregion

        #region Delete Tax
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? taxPK)
        {
            ITaxesRepository taxesRepository = new TaxesRepository(db);
            if (taxPK != null)
            {
                Tax tax = taxesRepository.GetTaxByPK((int)taxPK);

                tax.Deleted = true;

                taxesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", tax.TaxPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
