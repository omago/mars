using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.ChangeTypeModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class ChangeTypeController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public ChangeTypeController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IChangeTypesRepository changeTypesRepository = new ChangeTypesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "ChangeTypePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<ChangeType> changeTypes = changeTypesRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                changeTypes = changeTypes.Where(c => c.Name.Contains(searchString));
            }

            changeTypes = changeTypes.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = changeTypesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = changeTypesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("ChangeType?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", changeTypes.ToList());
            }

        }

        #endregion

        #region Add new ChangeType

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(ChangeTypeView changeTypeView)
        {
            if (ModelState.IsValid)
            {
                IChangeTypesRepository changeTypesRepository = new ChangeTypesRepository(db);
                ChangeType changeType = new ChangeType();

                changeTypeView.ConvertTo(changeTypeView, changeType);

                changeTypesRepository.Add(changeType);
                changeTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", changeType.ChangeTypePK);

                return RedirectToAction("Index", "ChangeType");
            }
            else
            {
                return View(changeTypeView);
            }
        }

        #endregion

        #region Edit ChangeType

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? changeTypePK)
        {
            if (changeTypePK != null)
            {
                IChangeTypesRepository changeTypesRepository = new ChangeTypesRepository(db);
                ChangeType changeType = changeTypesRepository.GetChangeTypeByPK((int)changeTypePK);
                ChangeTypeView changeTypeView = new ChangeTypeView();

                changeTypeView.ConvertFrom(changeType, changeTypeView);

                return View(changeTypeView);
            }
            else
            {
                return RedirectToAction("Index", "ChangeType");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(ChangeTypeView changeTypeModel)
        {
            if (ModelState.IsValid)
            {
                IChangeTypesRepository changeTypesRepository = new ChangeTypesRepository(db);
                ChangeType changeType = changeTypesRepository.GetChangeTypeByPK((int)changeTypeModel.ChangeTypePK);
                changeTypeModel.ConvertTo(changeTypeModel, changeType);

                changeTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", changeType.ChangeTypePK);

                return RedirectToAction("Index", "ChangeType");
            }
            else
            {
                return View(changeTypeModel);
            }
        }

        #endregion

        #region Delete ChangeType
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? changeTypePK)
        {
            IChangeTypesRepository changeTypesRepository = new ChangeTypesRepository(db);
            if (changeTypePK != null)
            {
                ChangeType changeType = changeTypesRepository.GetChangeTypeByPK((int)changeTypePK);

                changeType.Deleted = true;

                changeTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", changeType.ChangeTypePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
