using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.CountryModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class CountryController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public CountryController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ICountriesRepository countriesRepository = new CountriesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "CountryPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<Country> countries = countriesRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                countries = countries.Where(c => c.Name.Contains(searchString) || c.NameEn.Contains(searchString) || c.Citizenship.Contains(searchString));
            }

            countries = countries.Page(page, pageSize);

            ViewData["numberOfRecords"] = countriesRepository.GetValid().Count();

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Country?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", countries.ToList());
            }

        }

        #endregion

        #region Add new Country

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(CountryView countryView)
        {
            if (ModelState.IsValid)
            {
                ICountriesRepository countriesRepository = new CountriesRepository(db);
                Country country = new Country();

                countryView.ConvertTo(countryView, country);

                countriesRepository.Add(country);
                countriesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", country.CountryPK);

                return RedirectToAction("Index", "Country");
            }
            else
            {
                return View(countryView);
            }
        }

        #endregion

        #region Edit Country

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? countryPK)
        {
            if (countryPK != null)
            {
                ICountriesRepository countriesRepository = new CountriesRepository(db);

                Country country = countriesRepository.GetCountryByPK((int)countryPK);

                CountryView countryView = new CountryView();
                countryView.ConvertFrom(country, countryView);

                return View(countryView);
            }
            else
            {
                return RedirectToAction("Index", "Country");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(CountryView countryView)
        {
            if (ModelState.IsValid)
            {
                ICountriesRepository countriesRepository = new CountriesRepository(db);
                Country country = countriesRepository.GetCountryByPK((int)countryView.CountryPK);

                countryView.ConvertTo(countryView, country);

                countriesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", country.CountryPK);

                return RedirectToAction("Index", "Country");
            }
            else
            {
                return View(countryView);
            }
        }

        #endregion

        #region Delete Country

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? countryPK)
        {
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            if (countryPK != null)
            {
                Country country = countriesRepository.GetCountryByPK((int)countryPK);

                country.Deleted = true;

                countriesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", country.CountryPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
