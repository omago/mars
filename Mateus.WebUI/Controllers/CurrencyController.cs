using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.CurrencyModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class CurrencyController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public CurrencyController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ICurrenciesRepository currenciesRepository = new CurrenciesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "CurrencyPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<Currency> currencies = currenciesRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                currencies = currencies.Where(c => c.Name.Contains(searchString));
            }

            currencies = currencies.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = currenciesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = currenciesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Currency?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", currencies.ToList());
            }

        }

        #endregion

        #region Add new Currency

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(CurrencyView currencyView)
        {
            if (ModelState.IsValid)
            {
                ICurrenciesRepository currenciesRepository = new CurrenciesRepository(db);
                Currency currency = new Currency();

                currencyView.ConvertTo(currencyView, currency);

                currenciesRepository.Add(currency);
                currenciesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", currency.CurrencyPK);

                return RedirectToAction("Index", "Currency");
            }
            else
            {
                return View(currencyView);
            }
        }

        #endregion

        #region Edit Currency

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? currencyPK)
        {
            if (currencyPK != null)
            {
                ICurrenciesRepository currenciesRepository = new CurrenciesRepository(db);
                Currency currency = currenciesRepository.GetCurrencyByPK((int)currencyPK);
                CurrencyView currencyView = new CurrencyView();

                currencyView.ConvertFrom(currency, currencyView);

                return View(currencyView);
            }
            else
            {
                return RedirectToAction("Index", "Currency");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(CurrencyView currencyModel)
        {
            if (ModelState.IsValid)
            {
                ICurrenciesRepository currenciesRepository = new CurrenciesRepository(db);
                Currency currency = currenciesRepository.GetCurrencyByPK((int)currencyModel.CurrencyPK);
                currencyModel.ConvertTo(currencyModel, currency);

                currenciesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", currency.CurrencyPK);

                return RedirectToAction("Index", "Currency");
            }
            else
            {
                return View(currencyModel);
            }
        }

        #endregion

        #region Delete Currency
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? currencyPK)
        {
            ICurrenciesRepository currenciesRepository = new CurrenciesRepository(db);
            if (currencyPK != null)
            {
                Currency currency = currenciesRepository.GetCurrencyByPK((int)currencyPK);

                currency.Deleted = true;

                currenciesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", currency.CurrencyPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
