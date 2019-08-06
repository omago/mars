using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.PostalOfficeModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class PostalOfficeController : Controller
    {
        #region Initalizers

        // GET: /PostalOffice/
        Mateus_wcEntities db = null;
        public PostalOfficeController()
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

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "PostalOfficePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<PostalOfficeView> postalOffices = PostalOfficeView.GetPostalOfficeView(postalOfficesRepository.GetValid(), countiesRepository.GetValid(), countriesRepository.GetValid())  
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                postalOffices = postalOffices.Where(c => c.Name.Contains(searchString));
            }

            postalOffices = postalOffices.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = postalOfficesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = postalOfficesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("PostalOffice?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", postalOffices.ToList());
            }

        }

        #endregion

        #region Add new PostalOffice

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            PostalOfficeView postalOfficeView = new PostalOfficeView();

            // set default country to Croatia
            postalOfficeView.CountryFK = 81;

            postalOfficeView.BindDDLs(postalOfficeView, db);

            return View(postalOfficeView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(PostalOfficeView postalOfficeView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
                PostalOffice postalOffice = new PostalOffice();

                postalOfficeView.ConvertTo(postalOfficeView, postalOffice);

                postalOfficesRepository.Add(postalOffice);
                postalOfficesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", postalOffice.PostalOfficePK);

                return RedirectToAction("Index", "PostalOffice");
            }
            else
            {
                postalOfficeView.BindDDLs(postalOfficeView, db);

                return View(postalOfficeView);
            }
        }

        #endregion

        #region Edit PostalOffice

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? postalOfficePK)
        {
            if (postalOfficePK != null)
            {
                IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
                PostalOffice postalOffice = postalOfficesRepository.GetPostalOfficeByPK((int)postalOfficePK);
                PostalOfficeView postalOfficeView = new PostalOfficeView();

                postalOfficeView.ConvertFrom(postalOffice, postalOfficeView, db);
                postalOfficeView.BindDDLs(postalOfficeView, db);

                return View(postalOfficeView);
            }
            else
            {
                return RedirectToAction("Index", "PostalOffice");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(PostalOfficeView postalOfficeView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
                PostalOffice postalOffice = postalOfficesRepository.GetPostalOfficeByPK((int)postalOfficeView.PostalOfficePK);
                postalOfficeView.ConvertTo(postalOfficeView, postalOffice);

                postalOfficesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", postalOffice.PostalOfficePK);

                return RedirectToAction("Index", "PostalOffice");
            }
            else
            {
                postalOfficeView.BindDDLs(postalOfficeView, db);
                return View(postalOfficeView);
            }
        }

        #endregion

        #region Delete PostalOffice
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? postalOfficePK)
        {
            IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
            if (postalOfficePK != null)
            {
                PostalOffice postalOffice = postalOfficesRepository.GetPostalOfficeByPK((int)postalOfficePK);

                postalOffice.Deleted = true;

                postalOfficesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", postalOffice.PostalOfficePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
