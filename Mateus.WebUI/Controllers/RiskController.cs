using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.RiskModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class RiskController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public RiskController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IRisksRepository risksRepository = new RisksRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "RiskPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<Risk> risks = risksRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                risks = risks.Where(c => c.Name.Contains(searchString));
            }

            risks = risks.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = risksRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = risksRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Risk?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", risks.ToList());
            }

        }

        #endregion

        #region Add new Risk

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(RiskView riskView)
        {
            if (ModelState.IsValid)
            {
                IRisksRepository risksRepository = new RisksRepository(db);
                Risk risk = new Risk();

                riskView.ConvertTo(riskView, risk);

                risksRepository.Add(risk);
                risksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", risk.RiskPK);

                return RedirectToAction("Index", "Risk");
            }
            else
            {
                return View(riskView);
            }
        }

        #endregion

        #region Edit Risk

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? riskPK)
        {
            if (riskPK != null)
            {
                IRisksRepository risksRepository = new RisksRepository(db);
                Risk risk = risksRepository.GetRiskByPK((int)riskPK);
                RiskView riskView = new RiskView();

                riskView.ConvertFrom(risk, riskView);

                return View(riskView);
            }
            else
            {
                return RedirectToAction("Index", "Risk");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(RiskView riskView)
        {
            if (ModelState.IsValid)
            {
                IRisksRepository risksRepository = new RisksRepository(db);
                Risk risk = risksRepository.GetRiskByPK((int)riskView.RiskPK);
                riskView.ConvertTo(riskView, risk);

                risksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", risk.RiskPK);

                return RedirectToAction("Index", "Risk");
            }
            else
            {
                return View(riskView);
            }
        }

        #endregion

        #region Delete Risk
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? riskPK)
        {
            IRisksRepository risksRepository = new RisksRepository(db);
            if (riskPK != null)
            {
                Risk risk = risksRepository.GetRiskByPK((int)riskPK);

                risk.Deleted = true;

                risksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", risk.RiskPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
