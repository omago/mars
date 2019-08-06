using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.AssessmentTypeModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class AssessmentTypeController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public AssessmentTypeController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
            IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "AssessmentTypePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<AssessmentTypeView> assessmentTypes = AssessmentTypeView.GetAssessmentTypeListView(assessmentTypesRepository.GetValid(), assessmentGroupsRepository.GetValid())
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                assessmentTypes = assessmentTypes.Where(c => c.Name.Contains(searchString));
            }

            assessmentTypes = assessmentTypes.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = assessmentTypesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = assessmentTypesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("AssessmentType?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", assessmentTypes.ToList());
            }

        }

        #endregion

        #region Add new AssessmentType

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(AssessmentTypeView assessmentTypeView)
        {
            if (ModelState.IsValid)
            {
                IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
                AssessmentType assessmentType = new AssessmentType();

                assessmentTypeView.ConvertTo(assessmentTypeView, assessmentType);

                assessmentTypesRepository.Add(assessmentType);
                assessmentTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", assessmentType.AssessmentTypePK);

                return RedirectToAction("Index", "AssessmentType");
            }
            else
            {
                return View(assessmentTypeView);
            }
        }

        #endregion

        #region Edit AssessmentType

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? assessmentTypePK)
        {
            if (assessmentTypePK != null)
            {
                IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
                AssessmentType assessmentType = assessmentTypesRepository.GetAssessmentTypeByPK((int)assessmentTypePK);
                AssessmentTypeView assessmentTypeView = new AssessmentTypeView();

                assessmentTypeView.ConvertFrom(assessmentType, assessmentTypeView);

                return View(assessmentTypeView);
            }
            else
            {
                return RedirectToAction("Index", "AssessmentType");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(AssessmentTypeView assessmentTypeModel)
        {
            if (ModelState.IsValid)
            {
                IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
                AssessmentType assessmentType = assessmentTypesRepository.GetAssessmentTypeByPK((int)assessmentTypeModel.AssessmentTypePK);
                assessmentTypeModel.ConvertTo(assessmentTypeModel, assessmentType);

                assessmentTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", assessmentType.AssessmentTypePK);

                return RedirectToAction("Index", "AssessmentType");
            }
            else
            {
                return View(assessmentTypeModel);
            }
        }

        #endregion

        #region Delete AssessmentType
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? assessmentTypePK)
        {
            IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
            if (assessmentTypePK != null)
            {
                AssessmentType assessmentType = assessmentTypesRepository.GetAssessmentTypeByPK((int)assessmentTypePK);

                assessmentType.Deleted = true;

                assessmentTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", assessmentType.AssessmentTypePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}
