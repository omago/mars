using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.ContractValidityModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class ContractValidityController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public ContractValidityController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IContractValiditiesRepository contractValiditiesRepository = new ContractValiditiesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "ContractValidityPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<ContractValidity> contractValidities = contractValiditiesRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                contractValidities = contractValidities.Where(c => c.Name.Contains(searchString));
            }

            contractValidities = contractValidities.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = contractValiditiesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = contractValiditiesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("ContractValidity?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", contractValidities.ToList());
            }

        }

        #endregion

        #region Add new ContractValidity

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(ContractValidityView contractValidityView)
        {
            if (ModelState.IsValid)
            {
                IContractValiditiesRepository contractValiditiesRepository = new ContractValiditiesRepository(db);
                ContractValidity contractValidity = new ContractValidity();

                contractValidityView.ConvertTo(contractValidityView, contractValidity);

                contractValiditiesRepository.Add(contractValidity);
                contractValiditiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", contractValidity.ContractValidityPK);

                return RedirectToAction("Index", "ContractValidity");
            }
            else
            {
                return View(contractValidityView);
            }
        }

        #endregion

        #region Edit ContractValidity

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? contractValidityPK)
        {
            if (contractValidityPK != null)
            {
                IContractValiditiesRepository contractValiditiesRepository = new ContractValiditiesRepository(db);
                ContractValidity contractValidity = contractValiditiesRepository.GetContractValidityByPK((int)contractValidityPK);
                ContractValidityView contractValidityView = new ContractValidityView();

                contractValidityView.ConvertFrom(contractValidity, contractValidityView);

                return View(contractValidityView);
            }
            else
            {
                return RedirectToAction("Index", "ContractValidity");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(ContractValidityView contractValidityModel)
        {
            if (ModelState.IsValid)
            {
                IContractValiditiesRepository contractValiditiesRepository = new ContractValiditiesRepository(db);
                ContractValidity contractValidity = contractValiditiesRepository.GetContractValidityByPK((int)contractValidityModel.ContractValidityPK);
                contractValidityModel.ConvertTo(contractValidityModel, contractValidity);

                contractValiditiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", contractValidity.ContractValidityPK);

                return RedirectToAction("Index", "ContractValidity");
            }
            else
            {
                return View(contractValidityModel);
            }
        }

        #endregion

        #region Delete ContractValidity
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? contractValidityPK)
        {
            IContractValiditiesRepository contractValiditiesRepository = new ContractValiditiesRepository(db);
            if (contractValidityPK != null)
            {
                ContractValidity contractValidity = contractValiditiesRepository.GetContractValidityByPK((int)contractValidityPK);

                contractValidity.Deleted = true;

                contractValiditiesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", contractValidity.ContractValidityPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
