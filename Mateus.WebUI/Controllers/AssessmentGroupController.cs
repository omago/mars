using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.AssessmentGroupModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using PITFramework.Support;

namespace Mateus.Controllers
{
    public class AssessmentGroupController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public AssessmentGroupController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
            IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
            IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "AssessmentGroupPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<AssessmentGroupView> assessmentGroups = AssessmentGroupView.GetAssessmentGroupView(assessmentGroupsRepository.GetValid(), assessmentTypesRepository.GetValid(), assessmentQuestionsRepository.GetValid())  
                                                        .OrderBy(ordering);

            //AssessmentTypes ddl
            ViewBag.AssessmentTypes = new SelectList(assessmentTypesRepository.GetValid().OrderBy("Name ASC").ToList(), "AssessmentTypePK", "Name", Request.QueryString["assessmentTypeFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            { 
                string searchString = Request.QueryString["searchString"].ToString();
                assessmentGroups = assessmentGroups.Where(c => c.Name.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["assessmentTypeFK"]))
            {
                int assessmentTypeFK = Convert.ToInt32(Request.QueryString["assessmentTypeFK"]);
                assessmentGroups = assessmentGroups.Where(c => c.AssessmentTypeFK == assessmentTypeFK);
            }

            assessmentGroups = assessmentGroups.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"])) {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = assessmentGroupsRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            } else {
                ViewData["numberOfRecords"] = assessmentGroupsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if(page > numberOfPages) {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("AssessmentGroup?" + url + "page=" + numberOfPages);
            } else {
                return View("Index", assessmentGroups.ToList());
            }
            
        }

        #endregion

        #region Add new AssessmentGroup

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            AssessmentGroupView assessmentGroupView = new AssessmentGroupView();
            assessmentGroupView.BindDDLs(assessmentGroupView, db);

            return View(assessmentGroupView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(AssessmentGroupView assessmentGroupView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                AssessmentGroup assessmentGroup = new AssessmentGroup();

                assessmentGroupView.ConvertTo(assessmentGroupView, assessmentGroup);

                assessmentGroupsRepository.Add(assessmentGroup);
                assessmentGroupsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", assessmentGroup.AssessmentGroupPK);

                return RedirectToAction("Index", "AssessmentGroup");
            }
            else
            {
                assessmentGroupView.BindDDLs(assessmentGroupView, db);

                return View(assessmentGroupView);
            }
        }

        #endregion

        #region Edit AssessmentGroup

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? assessmentGroupPK)
        {
            if (assessmentGroupPK != null)
            {
                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                AssessmentGroup assessmentGroup = assessmentGroupsRepository.GetAssessmentGroupByPK((int)assessmentGroupPK);
                AssessmentGroupView assessmentGroupView = new AssessmentGroupView();

                assessmentGroupView.ConvertFrom(assessmentGroup, assessmentGroupView);
                assessmentGroupView.BindDDLs(assessmentGroupView, db);

                return View(assessmentGroupView);
            }
            else
            {
                return RedirectToAction("Index", "AssessmentGroup");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(AssessmentGroupView assessmentGroupView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                AssessmentGroup assessmentGroup = assessmentGroupsRepository.GetAssessmentGroupByPK((int)assessmentGroupView.AssessmentGroupPK);

                assessmentGroupView.ConvertTo(assessmentGroupView, assessmentGroup);

                assessmentGroupsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", assessmentGroup.AssessmentGroupPK);

                return RedirectToAction("Index", "AssessmentGroup");
            }
            else
            {
                assessmentGroupView.BindDDLs(assessmentGroupView, db);

                return View(assessmentGroupView);
            }
        }

        #endregion

        #region Delete AssessmentGroup

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? assessmentGroupPK)
        {
            IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
            if (assessmentGroupPK != null)
            {
                AssessmentGroup assessmentGroup = assessmentGroupsRepository.GetAssessmentGroupByPK((int)assessmentGroupPK);

                assessmentGroup.Deleted = true;

                assessmentGroupsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", assessmentGroup.AssessmentGroupPK);
            }
           
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

    }
}
