using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.ContractModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class ContractController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public ContractController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IContractsRepository contractsRepository = new ContractsRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IAnnexContractsRepository annexContractsRepository = new AnnexContractsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "ContractPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<ContractView> contracts = ContractView.GetContractsView(contractsRepository.GetValid(), annexContractsRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities())
                                                        .OrderBy(ordering);

            //legalEntities ddl
            ViewBag.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name", Request.QueryString["legalEntityFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                contracts = contracts.Where(c => c.Name.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["legalEntityFK"]))
            {
                int legalEntityFK = Convert.ToInt32(Request.QueryString["legalEntityFK"]);
                contracts = contracts.Where(c => c.LegalEntityFK == legalEntityFK);
            }

            contracts = contracts.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = contractsRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = contractsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Contract?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", contracts.ToList());
            }

        }

        #endregion

        #region Add new Contract

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? legalEntityFK)
        {
            ContractView contractView = new ContractView();

            if (legalEntityFK != null)
            {
                TempData["legalEntityFK"] = legalEntityFK;
                
                contractView.LegalEntityFK = (int)legalEntityFK;

                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                contractView.LegalEntityName = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityFK).Name;
            }

            contractView.BindDDLs(contractView, db);

            return View(contractView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(ContractView contractView)
        {
            if (ModelState.IsValid)
            {
                IContractsRepository contractsRepository = new ContractsRepository(db);
                Contract contract = new Contract();

                contractView.ConvertTo(contractView, contract);

                contractsRepository.Add(contract);
                contractsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", contract.ContractPK);

                if (TempData["legalEntityFK"] != null) return RedirectToAction("Index", "LegalEntity");
                else return RedirectToAction("Index", "Contract");
            }
            else
            {
                contractView.BindDDLs(contractView, db);

                return View(contractView);
            }
        }

        #endregion

        #region Edit Contract

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? contractPK)
        {
            if (contractPK != null)
            {
                IContractsRepository contractsRepository = new ContractsRepository(db);
                Contract contract = contractsRepository.GetContractByPK((int)contractPK);
                ContractView contractView = new ContractView();

                contractView.ConvertFrom(contract, contractView, db);
                contractView.BindDDLs(contractView, db);

                return View(contractView);
            }
            else
            {
                return RedirectToAction("Index", "Contract");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(ContractView contractView)
        {
            if (ModelState.IsValid)
            {
                IContractsRepository contractsRepository = new ContractsRepository(db);

                Contract contract = contractsRepository.GetContractByPK((int)contractView.ContractPK);
                contractView.ConvertTo(contractView, contract);

                contractsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", contract.ContractPK);

                return RedirectToAction("Index", "Contract");
            }
            else
            {
                contractView.BindDDLs(contractView, db);

                return View(contractView);
            }
        }

        #endregion

        #region Delete Contract
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? contractPK)
        {
            IContractsRepository contractsRepository = new ContractsRepository(db);
            if (contractPK != null)
            {
                Contract contract = contractsRepository.GetContractByPK((int)contractPK);

                contract.Deleted = true;

                contractsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", contract.ContractPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion


    }
}
