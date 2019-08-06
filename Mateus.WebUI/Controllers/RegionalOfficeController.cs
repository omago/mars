using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.RegionalOfficeModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class RegionalOfficeController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public RegionalOfficeController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "RegionalOfficePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<RegionalOffice> regionalOffices = regionalOfficesRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                regionalOffices = regionalOffices.Where(c => c.Name.Contains(searchString));
            }

            regionalOffices = regionalOffices.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = regionalOfficesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = regionalOfficesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("RegionalOffice?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", regionalOffices.ToList());
            }

        }

        #endregion

        #region Add new RegionalOffice

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(RegionalOfficeView regionalOfficeView)
        {
            if (ModelState.IsValid)
            {
                IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);
                RegionalOffice regionalOffice = new RegionalOffice();

                regionalOfficeView.ConvertTo(regionalOfficeView, regionalOffice);

                regionalOfficesRepository.Add(regionalOffice);
                regionalOfficesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", regionalOffice.RegionalOfficePK);

                return RedirectToAction("Index", "RegionalOffice");
            }
            else
            {
                return View(regionalOfficeView);
            }
        }

        #endregion

        #region Edit RegionalOffice

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? regionalOfficePK)
        {
            if (regionalOfficePK != null)
            {
                IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);
                RegionalOffice regionalOffice = regionalOfficesRepository.GetRegionalOfficeByPK((int)regionalOfficePK);
                RegionalOfficeView regionalOfficeView = new RegionalOfficeView();

                regionalOfficeView.ConvertFrom(regionalOffice, regionalOfficeView);

                return View(regionalOfficeView);
            }
            else
            {
                return RedirectToAction("Index", "RegionalOffice");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(RegionalOfficeView regionalOfficeModel)
        {
            if (ModelState.IsValid)
            {
                IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);
                RegionalOffice regionalOffice = regionalOfficesRepository.GetRegionalOfficeByPK((int)regionalOfficeModel.RegionalOfficePK);
                regionalOfficeModel.ConvertTo(regionalOfficeModel, regionalOffice);

                regionalOfficesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", regionalOffice.RegionalOfficePK);

                return RedirectToAction("Index", "RegionalOffice");
            }
            else
            {
                return View(regionalOfficeModel);
            }
        }

        #endregion

        #region Delete RegionalOffice
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? regionalOfficePK)
        {
            IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);
            if (regionalOfficePK != null)
            {
                RegionalOffice regionalOffice = regionalOfficesRepository.GetRegionalOfficeByPK((int)regionalOfficePK);

                regionalOffice.Deleted = true;

                regionalOfficesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", regionalOffice.RegionalOfficePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
