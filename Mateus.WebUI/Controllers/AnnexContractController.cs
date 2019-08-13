using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.AnnexContractModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using PITFramework.Support;

namespace Mateus.Controllers
{
    public class AnnexContractController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public AnnexContractController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IAnnexContractsRepository annexContractsRepository = new AnnexContractsRepository(db);
            IContractsRepository contractsRepository = new ContractsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "AnnexContractPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<AnnexContractView> annexContracts = AnnexContractView.GetAnnexContractView(annexContractsRepository.GetValid(), contractsRepository.GetValid())
                                                        .OrderBy(ordering);
            //contracts ddl
            ViewBag.Contracts = new SelectList(contractsRepository.GetValid().OrderBy("Name ASC").ToList(), "ContractPK", "Name", Request.QueryString["contractFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                annexContracts = annexContracts.Where(c => c.Name.Contains(searchString) || c.Number.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["contractFK"]))
            {
                int contractFK = Convert.ToInt32(Request.QueryString["contractFK"]);
                annexContracts = annexContracts.Where(c => c.ContractFK == contractFK);
            }

            annexContracts = annexContracts.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = annexContractsRepository.GetValid().Where(c => c.Name.Contains(searchString) || c.Number.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = annexContractsRepository.GetValid().Count();
            }

            ViewData["numberOfRecords"] = annexContractsRepository.GetValid().Count();

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("AnnexContract?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", annexContracts.ToList());
            }

        }

        #endregion

        #region Add new AnnexContract

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? contractFK)
        {
            AnnexContractView annexContractView = new AnnexContractView();
            annexContractView.BindDDLs(annexContractView, db);

            if (contractFK != null)
            {
                annexContractView.ContractFK = contractFK;
            }

            return View(annexContractView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(AnnexContractView annexContractView)
        {
            if (ModelState.IsValid)
            {
                IAnnexContractsRepository annexContractsRepository = new AnnexContractsRepository(db);
                AnnexContract annexContract = new AnnexContract();

                annexContractView.ConvertTo(annexContractView, annexContract);

                annexContractsRepository.Add(annexContract);
                annexContractsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", annexContract.AnnexContractPK);

                return RedirectToAction("Index", "AnnexContract");
            }
            else
            {
                annexContractView.BindDDLs(annexContractView, db);

                return View(annexContractView);
            }
        }

        #endregion

        #region Edit AnnexContract

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? annexContractPK)
        {
            if (annexContractPK != null)
            {
                IAnnexContractsRepository annexContractsRepository = new AnnexContractsRepository(db);
                AnnexContract annexContract = annexContractsRepository.GetAnnexContractByPK((int)annexContractPK);
                AnnexContractView annexContractView = new AnnexContractView();

                annexContractView.ConvertFrom(annexContract, annexContractView);
                annexContractView.BindDDLs(annexContractView, db);

                return View(annexContractView);
            }
            else
            {
                return RedirectToAction("Index", "AnnexContract");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(AnnexContractView annexContractView)
        {
            if (ModelState.IsValid)
            {
                IAnnexContractsRepository annexContractsRepository = new AnnexContractsRepository(db);

                AnnexContract annexContract = annexContractsRepository.GetAnnexContractByPK((int)annexContractView.AnnexContractPK);
                annexContractView.ConvertTo(annexContractView, annexContract);

                annexContractsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", annexContract.AnnexContractPK);

                return RedirectToAction("Index", "AnnexContract");
            }
            else
            {
                annexContractView.BindDDLs(annexContractView, db);
                
                return View(annexContractView);
            }
        }

        #endregion

        #region Delete AnnexContract
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? annexContractPK)
        {
            IAnnexContractsRepository annexContractsRepository = new AnnexContractsRepository(db);
            if (annexContractPK != null)
            {
                AnnexContract annexContract = annexContractsRepository.GetAnnexContractByPK((int)annexContractPK);

                annexContract.Deleted = true;

                annexContractsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", annexContract.AnnexContractPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
