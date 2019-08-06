using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.CityCommunityModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class CityCommunityController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public CityCommunityController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ICountiesRepository countiesRepository = new CountiesRepository(db);
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "CityCommunityPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<CityCommunityView> citiesCommunities = CityCommunityView.GetCityCommunityView(citiesCommunitiesRepository.GetValid(), countiesRepository.GetValid(), countriesRepository.GetValid())  
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                citiesCommunities = citiesCommunities.Where(c => c.Name.Contains(searchString));
            }

            citiesCommunities = citiesCommunities.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = citiesCommunitiesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = citiesCommunitiesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("CityCommunity?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", citiesCommunities.ToList());
            }

        }

        #endregion

        #region Add new CityCommunity

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            CityCommunityView cityCommunityView = new CityCommunityView();

            // set default country to Croatia
            cityCommunityView.CountryFK = 81;

            cityCommunityView.BindDDLs(cityCommunityView, db);

            return View(cityCommunityView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(CityCommunityView cityCommunityView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);
                CityCommunity cityCommunity = new CityCommunity();

                cityCommunityView.ConvertTo(cityCommunityView, cityCommunity);

                citiesCommunitiesRepository.Add(cityCommunity);
                citiesCommunitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", cityCommunity.CityCommunityPK);

                return RedirectToAction("Index", "CityCommunity");
            }
            else
            {
                cityCommunityView.BindDDLs(cityCommunityView, db);

                return View(cityCommunityView);
            }
        }

        #endregion

        #region Edit CityCommunity

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? cityCommunityPK)
        {
            if (cityCommunityPK != null)
            {
                ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);
                CityCommunity cityCommunity = citiesCommunitiesRepository.GetCityCommunityByPK((int)cityCommunityPK);
                CityCommunityView cityCommunityView = new CityCommunityView();

                cityCommunityView.ConvertFrom(cityCommunity, cityCommunityView, db);
                cityCommunityView.BindDDLs(cityCommunityView, db);

                return View(cityCommunityView);
            }
            else
            {
                return RedirectToAction("Index", "CityCommunity");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(CityCommunityView cityCommunityView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);
                CityCommunity cityCommunity = citiesCommunitiesRepository.GetCityCommunityByPK((int)cityCommunityView.CityCommunityPK);
                cityCommunityView.ConvertTo(cityCommunityView, cityCommunity);

                citiesCommunitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", cityCommunity.CityCommunityPK);

                return RedirectToAction("Index", "CityCommunity");
            }
            else
            {
                cityCommunityView.BindDDLs(cityCommunityView, db);
                return View(cityCommunityView);
            }
        }

        #endregion

        #region Delete CityCommunity

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? cityCommunityPK)
        {
            ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);
            if (cityCommunityPK != null)
            {
                CityCommunity cityCommunity = citiesCommunitiesRepository.GetCityCommunityByPK((int)cityCommunityPK);

                cityCommunity.Deleted = true;

                citiesCommunitiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", cityCommunity.CityCommunityPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}