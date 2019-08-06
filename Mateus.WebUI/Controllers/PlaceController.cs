using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.PlaceModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class PlaceController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public PlaceController()
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
            IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
            IPlacesRepository placesRepository = new PlacesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "PlacePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<PlaceView> places = PlaceView.GetPlaceView(placesRepository.GetValid(), postalOfficesRepository.GetValid(), countiesRepository.GetValid(), countriesRepository.GetValid())  
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                places = places.Where(c => c.Name.Contains(searchString));
            }

            places = places.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = placesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = placesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Place?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", places.ToList());
            }

        }

        #endregion

        #region Add new Place

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            PlaceView placeView = new PlaceView();

            // set default country to Croatia
            placeView.CountryFK = 81;

            placeView.BindDDLs(placeView, db);

            return View(placeView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(PlaceView placeView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IPlacesRepository placesRepository = new PlacesRepository(db);
                Place place = new Place();

                placeView.ConvertTo(placeView, place);

                placesRepository.Add(place);
                placesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", place.PlacePK);

                return RedirectToAction("Index", "Place");
            }
            else
            {
                placeView.BindDDLs(placeView, db);

                return View(placeView);
            }
        }

        #endregion

        #region Edit Place

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? placePK)
        {
            if (placePK != null)
            {
                IPlacesRepository placesRepository = new PlacesRepository(db);

                Place place = placesRepository.GetPlaceByPK((int)placePK);

                PlaceView placeView = new PlaceView();
                placeView.ConvertFrom(place, placeView, db);
                placeView.BindDDLs(placeView, db);

                return View(placeView);
            }
            else
            {
                return RedirectToAction("Index", "Place");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(PlaceView placeView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IPlacesRepository placesRepository = new PlacesRepository(db);
                Place place = placesRepository.GetPlaceByPK((int)placeView.PlacePK);
                placeView.ConvertTo(placeView, place);

                placesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", place.PlacePK);

                return RedirectToAction("Index", "Place");
            }
            else
            {
                placeView.BindDDLs(placeView, db);

                return View(placeView);
            }
        }

        #endregion

        #region Delete Place

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? placePK)
        {
            IPlacesRepository placesRepository = new PlacesRepository(db);
            if (placePK != null)
            {
                Place place = placesRepository.GetPlaceByPK((int)placePK);

                place.Deleted = true;

                placesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", place.PlacePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

    }
}
