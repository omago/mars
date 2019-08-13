using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.AssessmentQuestionModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using PITFramework.Support;

namespace Mateus.Controllers
{
    public class AssessmentQuestionController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public AssessmentQuestionController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);
            IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
            IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "AssessmentQuestionPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<AssessmentQuestionView> assessmentQuestions = AssessmentQuestionView.GetAssessmentQuestionView(assessmentQuestionsRepository.GetValid(), assessmentGroupsRepository.GetValid(), assessmentTypesRepository.GetValid())  
                                                        .OrderBy(ordering);

            //AssessmentGroups ddl
            ViewBag.AssessmentGroups = new SelectList(assessmentGroupsRepository.GetValid().OrderBy("Name ASC").ToList(), "AssessmentGroupPK", "Name", Request.QueryString["assessmentGroupFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            { 
                string searchString = Request.QueryString["searchString"].ToString();
                assessmentQuestions = assessmentQuestions.Where(c => c.Name.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["assessmentGroupFK"]))
            {
                int assessmentGroupFK = Convert.ToInt32(Request.QueryString["assessmentGroupFK"]);
                assessmentQuestions = assessmentQuestions.Where(c => c.AssessmentGroupFK == assessmentGroupFK);
            }

            assessmentQuestions = assessmentQuestions.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"])) {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = assessmentQuestionsRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            } else {
                ViewData["numberOfRecords"] = assessmentQuestionsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if(page > numberOfPages) {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("AssessmentQuestion?" + url + "page=" + numberOfPages);
            } else {
                return View("Index", assessmentQuestions.ToList());
            }
            
        }

        #endregion

        #region Add new AssessmentQuestion

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            AssessmentQuestionView assessmentQuestionView = new AssessmentQuestionView();
            assessmentQuestionView.BindDDLs(assessmentQuestionView, db);

            return View(assessmentQuestionView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(AssessmentQuestionView assessmentQuestionView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);
                AssessmentQuestion assessmentQuestion = new AssessmentQuestion();

                assessmentQuestionView.ConvertTo(assessmentQuestionView, assessmentQuestion);

                assessmentQuestionsRepository.Add(assessmentQuestion);
                assessmentQuestionsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", assessmentQuestion.AssessmentQuestionPK);

                return RedirectToAction("Index", "AssessmentQuestion");
            }
            else
            {
                assessmentQuestionView.BindDDLs(assessmentQuestionView, db);

                return View(assessmentQuestionView);
            }
        }

        #endregion

        #region Edit AssessmentQuestion

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? assessmentQuestionPK)
        {
            if (assessmentQuestionPK != null)
            {
                IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);
                AssessmentQuestion assessmentQuestion = assessmentQuestionsRepository.GetAssessmentQuestionByPK((int)assessmentQuestionPK);
                AssessmentQuestionView assessmentQuestionView = new AssessmentQuestionView();

                assessmentQuestionView.ConvertFrom(assessmentQuestion, assessmentQuestionView, db);
                assessmentQuestionView.BindDDLs(assessmentQuestionView, db);

                return View(assessmentQuestionView);
            }
            else
            {
                return RedirectToAction("Index", "AssessmentQuestion");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(AssessmentQuestionView assessmentQuestionView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);
                AssessmentQuestion assessmentQuestion = assessmentQuestionsRepository.GetAssessmentQuestionByPK((int)assessmentQuestionView.AssessmentQuestionPK);

                assessmentQuestionView.ConvertTo(assessmentQuestionView, assessmentQuestion);

                assessmentQuestionsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", assessmentQuestion.AssessmentQuestionPK);

                return RedirectToAction("Index", "AssessmentQuestion");
            }
            else
            {
                assessmentQuestionView.BindDDLs(assessmentQuestionView, db);

                return View(assessmentQuestionView);
            }
        }

        #endregion

        #region Delete AssessmentQuestion

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? assessmentQuestionPK)
        {
            IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);
            if (assessmentQuestionPK != null)
            {
                AssessmentQuestion assessmentQuestion = assessmentQuestionsRepository.GetAssessmentQuestionByPK((int)assessmentQuestionPK);

                assessmentQuestion.Deleted = true;

                assessmentQuestionsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", assessmentQuestion.AssessmentQuestionPK);
            }
           
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

    }
}
