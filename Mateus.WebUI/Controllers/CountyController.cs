using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.CountyModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class CountyController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public CountyController()
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

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "CountyPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<CountyView> counties = CountyView.GetCountyView(countiesRepository.GetValid(), countriesRepository.GetValid())  
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            { 
                string searchString = Request.QueryString["searchString"].ToString();
                counties = counties.Where(c => c.Name.Contains(searchString));
            }

            counties = counties.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"])) {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = countiesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            } else {
                ViewData["numberOfRecords"] = countiesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if(page > numberOfPages) {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("County?" + url + "page=" + numberOfPages);
            } else {
                return View("Index", counties.ToList());
            }
            
        }

        #endregion

        #region Add new County

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            CountyView countyView = new CountyView();

            countyView.BindDDLs(countyView, db);

            return View(countyView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(CountyView countyView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                County county = new County();

                countyView.ConvertTo(countyView, county);

                countiesRepository.Add(county);
                countiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", county.CountyPK);

                return RedirectToAction("Index", "County");
            }
            else
            {
                countyView.BindDDLs(countyView, db);

                return View(countyView);
            }
        }

        #endregion

        #region Edit County

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? countyPK)
        {
            if (countyPK != null)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                County county = countiesRepository.GetCountyByPK((int)countyPK);
                CountyView countyView = new CountyView();

                countyView.ConvertFrom(county, countyView);
                countyView.BindDDLs(countyView, db);

                return View(countyView);
            }
            else
            {
                return RedirectToAction("Index", "County");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(CountyView countyView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                County county = countiesRepository.GetCountyByPK((int)countyView.CountyPK);

                countyView.ConvertTo(countyView, county);

                countiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", county.CountyPK);

                return RedirectToAction("Index", "County");
            }
            else
            {
                countyView.BindDDLs(countyView, db);

                return View(countyView);
            }
        }

        #endregion

        #region Delete County

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? countyPK)
        {
            ICountiesRepository countiesRepository = new CountiesRepository(db);
            if (countyPK != null)
            {
                County county = countiesRepository.GetCountyByPK((int)countyPK);

                county.Deleted = true;

                countiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", county.CountyPK);
            }
           
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

    }
}
