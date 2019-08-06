using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.BussinesShareBurdenModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class BussinesShareBurdenController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public BussinesShareBurdenController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IBussinesShareBurdensRepository bussinesShareBurdensRepository = new BussinesShareBurdensRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "BussinesShareBurdenPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<BussinesShareBurden> bussinesShareBurdens = bussinesShareBurdensRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                bussinesShareBurdens = bussinesShareBurdens.Where(c => c.Name.Contains(searchString));
            }

            bussinesShareBurdens = bussinesShareBurdens.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = bussinesShareBurdensRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = bussinesShareBurdensRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("BussinesShareBurden?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", bussinesShareBurdens.ToList());
            }

        }

        #endregion

        #region Add new BussinesShareBurden

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(BussinesShareBurdenView bussinesShareBurdenView)
        {
            if (ModelState.IsValid)
            {
                IBussinesShareBurdensRepository bussinesShareBurdensRepository = new BussinesShareBurdensRepository(db);
                BussinesShareBurden bussinesShareBurden = new BussinesShareBurden();

                bussinesShareBurdenView.ConvertTo(bussinesShareBurdenView, bussinesShareBurden);

                bussinesShareBurdensRepository.Add(bussinesShareBurden);
                bussinesShareBurdensRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", bussinesShareBurden.BussinesShareBurdenPK);

                return RedirectToAction("Index", "BussinesShareBurden");
            }
            else
            {
                return View(bussinesShareBurdenView);
            }
        }

        #endregion

        #region Edit BussinesShareBurden

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? bussinesShareBurdenPK)
        {
            if (bussinesShareBurdenPK != null)
            {
                IBussinesShareBurdensRepository bussinesShareBurdensRepository = new BussinesShareBurdensRepository(db);
                BussinesShareBurden bussinesShareBurden = bussinesShareBurdensRepository.GetBussinesShareBurdenByPK((int)bussinesShareBurdenPK);
                BussinesShareBurdenView bussinesShareBurdenView = new BussinesShareBurdenView();

                bussinesShareBurdenView.ConvertFrom(bussinesShareBurden, bussinesShareBurdenView);

                return View(bussinesShareBurdenView);
            }
            else
            {
                return RedirectToAction("Index", "BussinesShareBurden");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(BussinesShareBurdenView bussinesShareBurdenModel)
        {
            if (ModelState.IsValid)
            {
                IBussinesShareBurdensRepository bussinesShareBurdensRepository = new BussinesShareBurdensRepository(db);
                BussinesShareBurden bussinesShareBurden = bussinesShareBurdensRepository.GetBussinesShareBurdenByPK((int)bussinesShareBurdenModel.BussinesShareBurdenPK);
                bussinesShareBurdenModel.ConvertTo(bussinesShareBurdenModel, bussinesShareBurden);

                bussinesShareBurdensRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", bussinesShareBurden.BussinesShareBurdenPK);

                return RedirectToAction("Index", "BussinesShareBurden");
            }
            else
            {
                return View(bussinesShareBurdenModel);
            }
        }

        #endregion

        #region Delete BussinesShareBurden

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? bussinesShareBurdenPK)
        {
            IBussinesShareBurdensRepository bussinesShareBurdensRepository = new BussinesShareBurdensRepository(db);
            if (bussinesShareBurdenPK != null)
            {
                BussinesShareBurden bussinesShareBurden = bussinesShareBurdensRepository.GetBussinesShareBurdenByPK((int)bussinesShareBurdenPK);

                bussinesShareBurden.Deleted = true;

                bussinesShareBurdensRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", bussinesShareBurden.BussinesShareBurdenPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
