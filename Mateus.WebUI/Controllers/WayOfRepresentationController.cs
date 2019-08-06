using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.WayOfRepresentationModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class WayOfRepresentationController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public WayOfRepresentationController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IWaysOfRepresentationRepository waysOfRepresentationRepository = new WaysOfRepresentationRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "WayOfRepresentationPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<WayOfRepresentation> waysOfRepresentation = waysOfRepresentationRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                waysOfRepresentation = waysOfRepresentation.Where(c => c.Name.Contains(searchString));
            }

            waysOfRepresentation = waysOfRepresentation.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = waysOfRepresentationRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = waysOfRepresentationRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("WayOfRepresentation?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", waysOfRepresentation.ToList());
            }

        }

        #endregion

        #region Add new WayOfRepresentation

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(WayOfRepresentationView wayOfRepresentationView)
        {
            if (ModelState.IsValid)
            {
                IWaysOfRepresentationRepository waysOfRepresentationRepository = new WaysOfRepresentationRepository(db);
                WayOfRepresentation wayOfRepresentation = new WayOfRepresentation();

                wayOfRepresentationView.ConvertTo(wayOfRepresentationView, wayOfRepresentation);

                waysOfRepresentationRepository.Add(wayOfRepresentation);
                waysOfRepresentationRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", wayOfRepresentation.WayOfRepresentationPK);

                return RedirectToAction("Index", "WayOfRepresentation");
            }
            else
            {
                return View(wayOfRepresentationView);
            }
        }

        #endregion

        #region Edit WayOfRepresentation

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? wayOfRepresentationPK)
        {
            if (wayOfRepresentationPK != null)
            {
                IWaysOfRepresentationRepository waysOfRepresentationRepository = new WaysOfRepresentationRepository(db);
                WayOfRepresentation wayOfRepresentation = waysOfRepresentationRepository.GetWayOfRepresentationByPK((int)wayOfRepresentationPK);
                WayOfRepresentationView wayOfRepresentationView = new WayOfRepresentationView();

                wayOfRepresentationView.ConvertFrom(wayOfRepresentation, wayOfRepresentationView);

                return View(wayOfRepresentationView);
            }
            else
            {
                return RedirectToAction("Index", "WayOfRepresentation");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(WayOfRepresentationView wayOfRepresentationModel)
        {
            if (ModelState.IsValid)
            {
                IWaysOfRepresentationRepository waysOfRepresentationRepository = new WaysOfRepresentationRepository(db);
                WayOfRepresentation wayOfRepresentation = waysOfRepresentationRepository.GetWayOfRepresentationByPK((int)wayOfRepresentationModel.WayOfRepresentationPK);
                wayOfRepresentationModel.ConvertTo(wayOfRepresentationModel, wayOfRepresentation);

                waysOfRepresentationRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", wayOfRepresentation.WayOfRepresentationPK);

                return RedirectToAction("Index", "WayOfRepresentation");
            }
            else
            {
                return View(wayOfRepresentationModel);
            }
        }

        #endregion

        #region Delete WayOfRepresentation
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? wayOfRepresentationPK)
        {
            IWaysOfRepresentationRepository waysOfRepresentationRepository = new WaysOfRepresentationRepository(db);
            if (wayOfRepresentationPK != null)
            {
                WayOfRepresentation wayOfRepresentation = waysOfRepresentationRepository.GetWayOfRepresentationByPK((int)wayOfRepresentationPK);

                wayOfRepresentation.Deleted = true;

                waysOfRepresentationRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", wayOfRepresentation.WayOfRepresentationPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
