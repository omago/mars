using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.CommercialCourtModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class CommercialCourtController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public CommercialCourtController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            ICommercialCourtsRepository commercialCourtsRepository = new CommercialCourtsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "CommercialCourtPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<CommercialCourt> commercialCourts = commercialCourtsRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                commercialCourts = commercialCourts.Where(c => c.Name.Contains(searchString));
            }

            commercialCourts = commercialCourts.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = commercialCourtsRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = commercialCourtsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("CommercialCourt?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", commercialCourts.ToList());
            }

        }

        #endregion

        #region Add new CommercialCourt

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(CommercialCourtView commercialCourtView)
        {
            if (ModelState.IsValid)
            {
                ICommercialCourtsRepository commercialCourtsRepository = new CommercialCourtsRepository(db);
                CommercialCourt commercialCourt = new CommercialCourt();

                commercialCourtView.ConvertTo(commercialCourtView, commercialCourt);

                commercialCourtsRepository.Add(commercialCourt);
                commercialCourtsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", commercialCourt.CommercialCourtPK);

                return RedirectToAction("Index", "CommercialCourt");
            }
            else
            {
                return View(commercialCourtView);
            }
        }

        #endregion

        #region Edit CommercialCourt

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? commercialCourtPK)
        {
            if (commercialCourtPK != null)
            {
                ICommercialCourtsRepository commercialCourtsRepository = new CommercialCourtsRepository(db);
                CommercialCourt commercialCourt = commercialCourtsRepository.GetCommercialCourtByPK((int)commercialCourtPK);
                CommercialCourtView commercialCourtView = new CommercialCourtView();

                commercialCourtView.ConvertFrom(commercialCourt, commercialCourtView);

                return View(commercialCourtView);
            }
            else
            {
                return RedirectToAction("Index", "CommercialCourt");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(CommercialCourtView commercialCourtModel)
        {
            if (ModelState.IsValid)
            {
                ICommercialCourtsRepository commercialCourtsRepository = new CommercialCourtsRepository(db);
                CommercialCourt commercialCourt = commercialCourtsRepository.GetCommercialCourtByPK((int)commercialCourtModel.CommercialCourtPK);
                commercialCourtModel.ConvertTo(commercialCourtModel, commercialCourt);

                commercialCourtsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", commercialCourt.CommercialCourtPK);

                return RedirectToAction("Index", "CommercialCourt");
            }
            else
            {
                return View(commercialCourtModel);
            }
        }

        #endregion

        #region Delete CommercialCourt
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? commercialCourtPK)
        {
            ICommercialCourtsRepository commercialCourtsRepository = new CommercialCourtsRepository(db);
            if (commercialCourtPK != null)
            {
                CommercialCourt commercialCourt = commercialCourtsRepository.GetCommercialCourtByPK((int)commercialCourtPK);

                commercialCourt.Deleted = true;

                commercialCourtsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", commercialCourt.CommercialCourtPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
