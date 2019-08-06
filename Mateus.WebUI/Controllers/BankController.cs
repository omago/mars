using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.BankModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class BankController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public BankController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IBanksRepository banksRepository = new BanksRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "BankPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<Bank> banks = banksRepository.GetValid()
                                                    .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                banks = banks.Where(c => c.Name.Contains(searchString) || c.AccountNumber.Contains(searchString) || c.Swift.Contains(searchString) || c.Oib.Contains(searchString) || c.Iban.Contains(searchString));
            }

            banks = banks.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = banksRepository.GetValid().Where(c => c.Name.Contains(searchString) || c.AccountNumber.Contains(searchString) || c.Swift.Contains(searchString) || c.Oib.Contains(searchString) || c.Iban.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = banksRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Bank?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", banks.ToList());
            }

        }

        #endregion

        #region Add new Bank

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(BankView bankView)
        {
            if (ModelState.IsValid)
            {
                IBanksRepository banksRepository = new BanksRepository(db);
                Bank bank = new Bank();

                bankView.ConvertTo(bankView, bank);

                banksRepository.Add(bank);
                banksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", bank.BankPK);

                return RedirectToAction("Index", "Bank");
            }
            else
            {
                return View(bankView);
            }
        }

        #endregion

        #region Edit Bank

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? bankPK)
        {
            if (bankPK != null)
            {
                IBanksRepository banksRepository = new BanksRepository(db);
                Bank bank = banksRepository.GetBankByPK((int)bankPK);
                BankView bankView = new BankView();

                bankView.ConvertFrom(bank, bankView);

                return View(bankView);
            }
            else
            {
                return RedirectToAction("Index", "Bank");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(BankView bankModel)
        {
            if (ModelState.IsValid)
            {
                IBanksRepository banksRepository = new BanksRepository(db);
                Bank bank = banksRepository.GetBankByPK((int)bankModel.BankPK);
                bankModel.ConvertTo(bankModel, bank);

                banksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", bank.BankPK);

                return RedirectToAction("Index", "Bank");
            }
            else
            {
                return View(bankModel);
            }
        }

        #endregion

        #region Delete Bank
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? bankPK)
        {
            IBanksRepository banksRepository = new BanksRepository(db);
            if (bankPK != null)
            {
                Bank bank = banksRepository.GetBankByPK((int)bankPK);

                bank.Deleted = true;

                banksRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", bank.BankPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
