using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.LegalEntityOwnerModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using Mateus.Model.BussinesLogic.Views.AssessmentsModel;

namespace Mateus.Controllers
{
    public class AssessmentController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public AssessmentController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IAssessmentsRepository assessmentsRepository = new AssessmentsRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IRisksRepository risksRepository = new RisksRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "AssessmentPK";
            string ordering = sortColumn + " " + sortOrder;

            ordering = ordering.Trim();

            IQueryable<AssessmentsView> assessments = AssessmentsView.GetHomeView(assessmentsRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities(), risksRepository.GetValid())
                                                        .OrderBy(ordering);
            
            //legalEntities ddl
            ViewBag.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name", Request.QueryString["legalEntityFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["LegalEntityFK"]))
            {
                int legalEntityFK = Convert.ToInt32(Request.QueryString["LegalEntityFK"]);
                assessments = assessments.Where(a => a.LegalEntityFK == legalEntityFK);
            }

            int assessmentCount = 0;
            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                assessments = assessments.Where(c => c.AssessmentComment.Contains(searchString) || c.AssessmentDate.ToString().Contains(searchString) || c.LegalEntityName.Contains(searchString));
                assessmentCount = assessments.Count();
            }

            assessments = assessments.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = assessmentCount;
            }
            else
            {
                ViewData["numberOfRecords"] = assessmentsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Assessment?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", assessments.ToList());
            }

        }

        #endregion

        #region Add new Assessment

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? legalEntityFK)
        {
            TempData["legalEntityFK"] = legalEntityFK;

            AssessmentsView assessmentView = new AssessmentsView();
           
            // Assessment types ddl
            IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
            assessmentView.AssessmentTypes = new SelectList(assessmentTypesRepository.GetValid().ToList(), "AssessmentTypePK", "Name");
            
            //legalEntities ddl
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            if (legalEntityFK != null)
            {
                TempData["legalEntityFK"] = legalEntityFK;
                assessmentView.LegalEntityFK = (int)legalEntityFK;
                assessmentView.LegalEntityName = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityFK).Name;
            } 

            // Assessment types 
            IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
            IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);

            assessmentView.AssessmentsTypesView = AssessmentsView.FillQuiz(assessmentTypesRepository.GetValid(), assessmentGroupsRepository, assessmentQuestionsRepository, null);


            return View(assessmentView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(AssessmentsView assessmentView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IAssessmentsRepository assessmentsRepository = new AssessmentsRepository(db);
                Assessment assessment = new Assessment();

                assessmentView.ConvertTo(assessmentView, assessment);

                assessmentsRepository.Add(assessment);
                assessmentsRepository.SaveChanges();

                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);
                IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);

                assessmentView.AssessmentsTypesView = AssessmentsView.FillQuiz(assessmentTypesRepository.GetValid(), assessmentGroupsRepository, assessmentQuestionsRepository, form);

                // Fetching answered type
                AssessmentsTypeView atw = assessmentView.AssessmentsTypesView.Where(a => a.AssessmentType.AssessmentTypePK == assessmentView.AssessmentTypeFK).First();

                List<AssessmentAnswers> assessmentAnswers = AssessmentAnswerView.ExtractQuizAnswers(atw, assessment.AssessmentPK);

                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)assessmentView.LegalEntityFK);
                bool activityHighRisky = legalEntity.Activity.HighRisk != null && (bool)legalEntity.Activity.HighRisk ? true : false;
                bool activityLowRisky = legalEntity.Activity.LowRisk != null && (bool)legalEntity.Activity.LowRisk ? true : false;
                bool headquarterCountryRisky = legalEntity.Country.Risk != null && (bool)legalEntity.Country.Risk ? true : false; 
                    
                IRisksRepository risksRepository = new RisksRepository(db);
                if (activityHighRisky || headquarterCountryRisky) // check explicitly if related legalEntity activity is high risk
                {
                    assessment.RiskFK = risksRepository.GetRiskByName("Visok").RiskPK;
                }
                else if (activityLowRisky) // check explicitly if related legalEntity activity is low risk
                {
                    assessment.RiskFK = risksRepository.GetRiskByName("Nizak").RiskPK;
                }
                else // run assesment quiz
                {
                    ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
                    IQueryable<LegalEntityOwner> legalEntityOwnersTable = legalEntityOwnersRepository.GetValid();
                    ICountriesRepository countriesRepository = new CountriesRepository(db);

                    IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);

                    List<LegalEntityOwner> coList = legalEntityOwnersRepository.GetFirstLegalEntityOwnersForLegalEntity((int)assessmentView.LegalEntityFK).ToList();

                    legalEntityOwnersTable = LegalEntityOwnerView.GetLegalEntityOwnersForLegalEntity(Convert.ToInt32(assessmentView.LegalEntityFK), coList, legalEntityOwnersTable);

                    IQueryable<LegalEntity> legalEntityView = LegalEntityOwnerView.GetRelatedLegalEntities(legalEntityOwnersTable, legalEntitiesRepository.GetValid(), countriesRepository.GetValid(), activitiesRepository.GetValid());

                    // check owners risks
                    bool ownersResidenceCountryRisky = false;
                    bool ownersActivitiesHighlyRisky = false;
                    bool ownersActivitiesLowRisky = false;

                    if(legalEntityView.Count() > 0) 
                    {
                        if(legalEntityView.ToList().TrueForAll(le => le.Country.Risk == null || (le.Country.Risk != null && (bool)le.Country.Risk != true)) == false)
                        {
                            ownersResidenceCountryRisky = true;
                        }
                        
                        if(legalEntityView.ToList().TrueForAll(le => le.Activity == null || (le.Activity.HighRisk == null || (le.Activity.HighRisk != null && (bool)le.Activity.HighRisk != true))) == false)
                        {
                            ownersActivitiesHighlyRisky = true;
                        }

                        if(legalEntityView.ToList().TrueForAll(le => le.Activity == null || (le.Activity.LowRisk == null || (le.Activity.LowRisk != null && (bool)le.Activity.LowRisk != true))) == false)
                        {
                            ownersActivitiesLowRisky = true;
                        }
                    }

                    // check form 
                    bool allAnswersYes = false;
                    bool allAnswersNo = false;
                    bool mixedAnswers = false;

                    if(assessmentAnswers.TrueForAll(aa => aa.AssessmentAnswer != null && (bool)aa.AssessmentAnswer == true)) 
                    { 
                        allAnswersYes = true; 
                    }

                    if(assessmentAnswers.TrueForAll(aa => aa.AssessmentAnswer != null && (bool)aa.AssessmentAnswer == false)) 
                    { 
                        allAnswersNo = true; 
                    }

                    if(allAnswersYes == false && allAnswersNo == false) 
                    { 
                        mixedAnswers = true; 
                    }

                    // check general conditions
                    if(ownersActivitiesLowRisky) 
                    {
                        assessment.RiskFK = risksRepository.GetRiskByName("Nizak").RiskPK;
                    }
                    else if (allAnswersYes || ownersResidenceCountryRisky || ownersActivitiesHighlyRisky) 
                    {
                        assessment.RiskFK = risksRepository.GetRiskByName("Visok").RiskPK;
                    }
                    else if (allAnswersNo || mixedAnswers) 
                    {
                        assessment.RiskFK = risksRepository.GetRiskByName("Srednji").RiskPK;
                    }
                }

                assessmentsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", assessment.AssessmentPK);

                IAssessmentAnswersRepository assessmentAnswersRepository = new AssessmentAnswersRepository(db);

                if (assessmentAnswers.Count() > 0)
                {
                    assessmentAnswersRepository.AddAll(assessmentAnswers);
                    assessmentAnswersRepository.SaveChanges();
                }

                if (TempData["legalEntityFK"] != null) return RedirectToAction("Index", "LegalEntity");
                else return RedirectToAction("Index", "Assessment");
            }
            else
            {
                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);

                // Assessment types ddl
                IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
                assessmentView.AssessmentTypes = new SelectList(assessmentTypesRepository.GetValid().ToList(), "AssessmentTypePK", "Name");
                
                //legalEntities ddl
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                assessmentView.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().ToList(), "LegalEntityPK", "Name");
                
                assessmentView.AssessmentsTypesView = AssessmentsView.FillQuiz(assessmentTypesRepository.GetValid(), assessmentGroupsRepository, assessmentQuestionsRepository, form);
                
                return View(assessmentView);
            }
        }

        #endregion

        #region Edit Assessment

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? assessmentPK)
        {
            if (assessmentPK != null)
            {
                IAssessmentsRepository assessmentsRepository = new AssessmentsRepository(db);
                Assessment assessment = assessmentsRepository.GetAssessmentByPK((int)assessmentPK);
                AssessmentsView assessmentView = new AssessmentsView();

                assessmentView.ConvertFrom(assessment, assessmentView);

                IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
                assessmentView.AssessmentTypes = new SelectList(assessmentTypesRepository.GetValid().ToList(), "AssessmentTypePK", "Name");

                // Assessment types ddl
                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);

                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)assessment.LegalEntityFK);
                assessmentView.LegalEntityName = legalEntity.Name + " (" + legalEntity.OIB + ")";

                FormCollection form = new FormCollection();

                IAssessmentAnswersRepository assessmentAnswersRepository = new AssessmentAnswersRepository(db);
                IQueryable<AssessmentAnswers> assessmentAnswers = assessmentAnswersRepository.GetAssessmentAnswersByAssessment((int)assessmentPK);

                foreach(var assessmentAnswer in assessmentAnswers) 
                {
                    string answer = assessmentAnswer.AssessmentAnswer == null ? "NP": (bool)assessmentAnswer.AssessmentAnswer ? "Da" : "Ne";
                    form.Add("answer[" + assessmentAnswer.AssessmentQuestionFK + "]", answer);
                }

                assessmentView.AssessmentsTypesView = AssessmentsView.FillQuiz(assessmentTypesRepository.GetValid(), assessmentGroupsRepository, assessmentQuestionsRepository, form);

                return View(assessmentView);
            }
            else
            {
                return RedirectToAction("Index", "Assessment");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(AssessmentsView assessmentView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IAssessmentsRepository assessmentsRepository = new AssessmentsRepository(db);
                Assessment assessment = assessmentsRepository.GetAssessmentByPK((int)assessmentView.AssessmentPK);

                IAssessmentAnswersRepository assessmentAnswersRepository = new AssessmentAnswersRepository(db);
                
                // Delete old values
                assessmentAnswersRepository.Delete(a => a.AssessmentFK == assessment.AssessmentPK);

                // Add new values
                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);
                IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);

                assessmentView.AssessmentsTypesView = AssessmentsView.FillQuiz(assessmentTypesRepository.GetValid(), assessmentGroupsRepository, assessmentQuestionsRepository, form);

                // Fetching answered type
                AssessmentsTypeView atw = assessmentView.AssessmentsTypesView.Where(a => a.AssessmentType.AssessmentTypePK == assessmentView.AssessmentTypeFK).First();

                List<AssessmentAnswers> assessmentAnswers = AssessmentAnswerView.ExtractQuizAnswers(atw, assessment.AssessmentPK);

                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)assessmentView.LegalEntityFK);
                bool activityHighRisky = legalEntity.Activity.HighRisk != null && (bool)legalEntity.Activity.HighRisk ? true : false;
                bool activityLowRisky = legalEntity.Activity.LowRisk != null && (bool)legalEntity.Activity.LowRisk ? true : false;
                bool headquarterCountryRisky = legalEntity.Country.Risk != null && (bool)legalEntity.Country.Risk ? true : false; 
                    
                IRisksRepository risksRepository = new RisksRepository(db);
                if (activityHighRisky || headquarterCountryRisky) // check explicitly if related legalEntity activity is high risk
                {
                    assessmentView.RiskFK = risksRepository.GetRiskByName("Visok").RiskPK;
                }
                else if (activityLowRisky) // check explicitly if related legalEntity activity is low risk
                {
                    assessmentView.RiskFK = risksRepository.GetRiskByName("Nizak").RiskPK;
                }
                else // run assesment quiz
                {
                    ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
                    IQueryable<LegalEntityOwner> legalEntityOwnersTable = legalEntityOwnersRepository.GetValid();
                    ICountriesRepository countriesRepository = new CountriesRepository(db);

                    IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);

                    List<LegalEntityOwner> coList = legalEntityOwnersRepository.GetFirstLegalEntityOwnersForLegalEntity((int)assessmentView.LegalEntityFK).ToList();

                    legalEntityOwnersTable = LegalEntityOwnerView.GetLegalEntityOwnersForLegalEntity(Convert.ToInt32(assessmentView.LegalEntityFK), coList, legalEntityOwnersTable);

                    IQueryable<LegalEntity> legalEntityView = LegalEntityOwnerView.GetRelatedLegalEntities(legalEntityOwnersTable, legalEntitiesRepository.GetValid(), countriesRepository.GetValid(), activitiesRepository.GetValid());

                    // check owners risks
                    bool ownersResidenceCountryRisky = false;
                    bool ownersActivitiesHighlyRisky = false;
                    bool ownersActivitiesLowRisky = false;

                    if(legalEntityView.Count() > 0) 
                    {
                        if(legalEntityView.ToList().TrueForAll(le => le.Country.Risk == null || (le.Country.Risk != null && (bool)le.Country.Risk != true)) == false)
                        {
                            ownersResidenceCountryRisky = true;
                        }
                        
                        if(legalEntityView.ToList().TrueForAll(le => le.Activity == null || (le.Activity.HighRisk == null || (le.Activity.HighRisk != null && (bool)le.Activity.HighRisk != true))) == false)
                        {
                            ownersActivitiesHighlyRisky = true;
                        }

                        if(legalEntityView.ToList().TrueForAll(le => le.Activity == null || (le.Activity.LowRisk == null || (le.Activity.LowRisk != null && (bool)le.Activity.LowRisk != true))) == false)
                        {
                            ownersActivitiesLowRisky = true;
                        }
                    }

                    // check form 
                    bool allAnswersYes = false;
                    bool allAnswersNo = false;
                    bool mixedAnswers = false;

                    if(assessmentAnswers.TrueForAll(aa => aa.AssessmentAnswer != null && (bool)aa.AssessmentAnswer == true)) 
                    { 
                        allAnswersYes = true; 
                    }

                    if(assessmentAnswers.TrueForAll(aa => aa.AssessmentAnswer != null && (bool)aa.AssessmentAnswer == false)) 
                    { 
                        allAnswersNo = true; 
                    }

                    if(allAnswersYes == false && allAnswersNo == false) 
                    { 
                        mixedAnswers = true; 
                    }

                    // check general conditions
                    if(ownersActivitiesLowRisky) 
                    {
                        assessmentView.RiskFK = risksRepository.GetRiskByName("Nizak").RiskPK;
                    }
                    else if (allAnswersYes || ownersResidenceCountryRisky || ownersActivitiesHighlyRisky) 
                    {
                        assessmentView.RiskFK = risksRepository.GetRiskByName("Visok").RiskPK;
                    }
                    else if (allAnswersNo || mixedAnswers) 
                    {
                        assessmentView.RiskFK = risksRepository.GetRiskByName("Srednji").RiskPK;
                    }
                }

                if (assessmentAnswers.Count() > 0)
                {
                    assessmentAnswersRepository.AddAll(assessmentAnswers);
                    assessmentAnswersRepository.SaveChanges();
                }

                assessmentView.ConvertTo(assessmentView, assessment);
                assessmentsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", assessment.AssessmentPK);

                return RedirectToAction("Index", "Assessment");
            }
            else
            {
                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                IAssessmentQuestionsRepository assessmentQuestionsRepository = new AssessmentQuestionsRepository(db);

                //Assessment types ddl
                IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
                assessmentView.AssessmentTypes = new SelectList(assessmentTypesRepository.GetValid().ToList(), "AssessmentTypePK", "Name");
                
                //legalEntities ddl
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                assessmentView.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().ToList(), "LegalEntityPK", "Name");

                assessmentView.AssessmentsTypesView = AssessmentsView.FillQuiz(assessmentTypesRepository.GetValid(), assessmentGroupsRepository, assessmentQuestionsRepository, form);

                return View(assessmentView);
            }
        }

        #endregion

        #region Delete Assessment

        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? assessmentPK)
        {
            IAssessmentsRepository assessmentsRepository = new AssessmentsRepository(db);
            if (assessmentPK != null)
            {
                Assessment assessment = assessmentsRepository.GetAssessmentByPK((int)assessmentPK);

                assessment.Deleted = true;

                assessmentsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", assessment.AssessmentPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}