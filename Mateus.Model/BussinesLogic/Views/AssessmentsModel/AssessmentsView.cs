using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;

namespace Mateus.Model.BussinesLogic.Views.AssessmentsModel
{
    public class AssessmentsTypeView
    {
        public AssessmentType AssessmentType { get; set; }

        public IEnumerable<AssessmentsGroupView> AssessmentGroups { get; set; }

        public AssessmentsTypeView()
        {

        }
    }

    public class AssessmentsGroupView
    {
        public AssessmentGroup AssessmentGroup { get; set; }

        public IEnumerable<AssessmentQuestionView> AssessmentQuestions { get; set; }

        public AssessmentsGroupView()
        {

        }
    }

    public class AssessmentQuestionView
    {
        public AssessmentQuestion AssessmentQuestion { get; set; }

        public bool? Answer { get; set; }

        public AssessmentQuestionView()
        {

        }
    }

    public class AssessmentAnswerView
    {
        public static List<AssessmentAnswers> ExtractQuizAnswers(AssessmentsTypeView atw, int AssessmentPK) 
        {
            List<AssessmentAnswers> tmpAssessmentAnswers = new List<AssessmentAnswers>();
                
            foreach (var group in atw.AssessmentGroups)
            {
                foreach (var question in group.AssessmentQuestions)
                {
                    tmpAssessmentAnswers.Add(new AssessmentAnswers()
                    {
                        AssessmentQuestionFK = question.AssessmentQuestion.AssessmentQuestionPK,
                        AssessmentAnswer = question.Answer,
                        AssessmentFK = AssessmentPK
                    });
                }
            }

            return tmpAssessmentAnswers;
        }
    }

    public class AssessmentsView
    {
        public int AssessmentPK { get; set; }

        [Required(ErrorMessage = "Tip procjene je obavezan.")]
        public int? AssessmentTypeFK { get; set; }

        [Required(ErrorMessage = "Datum procjene je obavezan.")]
        public DateTime? AssessmentDate { get; set; }

        [Required(ErrorMessage = "Komentar procjene je obavezan.")]
        public string AssessmentComment { get; set; }

        [Required(ErrorMessage = "Tvrtka je obavezna.")]
        public int? LegalEntityFK { get; set; }

        public int? RiskFK { get; set; }

        public string LegalEntityName { get; set; }

        public string Risk { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<AssessmentsTypeView> AssessmentsTypesView { get; set; }
        public IEnumerable<SelectListItem> AssessmentTypes { get; set; }
        public IEnumerable<SelectListItem> LegalEntities { get; set; }

        public void ConvertFrom(Assessment assessment, AssessmentsView assessmentView)
        {
            assessmentView.AssessmentPK = assessment.AssessmentPK;
            assessmentView.LegalEntityFK = assessment.LegalEntityFK;
            assessmentView.AssessmentTypeFK = assessment.AssessmentTypeFK;
            assessmentView.RiskFK = assessment.RiskFK;
            assessmentView.AssessmentComment = assessment.AssessmentComment;
            assessmentView.AssessmentDate = assessment.AssessmentDate;
            assessmentView.Deleted = assessment.Deleted;
        }

        public void ConvertTo(AssessmentsView assessmentView, Assessment assessment)
        {
            assessment.AssessmentPK = assessmentView.AssessmentPK;
            assessment.LegalEntityFK = assessmentView.LegalEntityFK;
            assessment.AssessmentTypeFK = assessmentView.AssessmentTypeFK;
            assessment.RiskFK = assessmentView.RiskFK;
            assessment.AssessmentComment = assessmentView.AssessmentComment;
            assessment.AssessmentDate = assessmentView.AssessmentDate;
            assessment.Deleted = assessmentView.Deleted;
        }


        public static IQueryable<AssessmentsView> GetHomeView(IQueryable<Assessment> assessments, IQueryable<LegalEntity> legalEntities, IQueryable<Risk> risks)
        {
            IQueryable<AssessmentsView> assessmentsView = (from a in assessments
                                                           from c in legalEntities.Where(c1 => c1.LegalEntityPK == a.LegalEntityFK).DefaultIfEmpty()
                                                           select new AssessmentsView
                                                           {
                                                               AssessmentPK = a.AssessmentPK,
                                                               AssessmentDate = a.AssessmentDate,
                                                               AssessmentComment = a.AssessmentComment,
                                                               Deleted = a.Deleted,
                                                               LegalEntityName = c.Name,
                                                               LegalEntityFK = a.LegalEntityFK,
                                                               Risk = risks.Where(r => r.RiskPK == a.RiskFK).FirstOrDefault().Name
                                                           }).AsQueryable<AssessmentsView>();

            return assessmentsView;
        }

        public static List<AssessmentsTypeView> FillQuiz(
            IQueryable<AssessmentType> assessmentTypes, 
            IAssessmentGroupsRepository assessmentGroupsRepository,
            IAssessmentQuestionsRepository assessmentQuestionsRepository,
            FormCollection form) 
        {
            List<AssessmentsTypeView> tmpAssessmentsTypes = new List<AssessmentsTypeView>();

            foreach (var assessmentType in assessmentTypes)
            {
                List<AssessmentsGroupView> AssessmentsGroups = new List<AssessmentsGroupView>();
                AssessmentsTypeView AssessmentsTypeView = new AssessmentsTypeView();
                AssessmentsTypeView.AssessmentType = assessmentType;

                var assessmentGroups = assessmentGroupsRepository.GetAssessmentGroupsByType(assessmentType.AssessmentTypePK);
                foreach (var assessmentGroup in assessmentGroups)
                {
                    AssessmentsGroupView assessmentsGroupTmp = new AssessmentsGroupView();
                    assessmentsGroupTmp.AssessmentGroup = assessmentGroup;

                    List<AssessmentQuestion> assessmentsQuestions = assessmentQuestionsRepository.GetAssessmentQuestionsByGroup(assessmentGroup.AssessmentGroupPK).ToList();
                    List<AssessmentQuestionView> assessmentsQuestionsView = new List<AssessmentQuestionView>();
                    foreach (var assessmentQuestion in assessmentsQuestions)
                    {
                        AssessmentQuestionView assessmentQuestionTmp = new AssessmentQuestionView();
                        assessmentQuestionTmp.AssessmentQuestion = assessmentQuestion;
                        bool? answered = null;
                        if (form != null)
                        {
                            if (form["answer[" + assessmentQuestion.AssessmentQuestionPK + "]"] != null)
                            {
                                if (form["answer[" + assessmentQuestion.AssessmentQuestionPK + "]"].ToString() == "NP") answered = null;
                                else answered = form["answer[" + assessmentQuestion.AssessmentQuestionPK + "]"].ToString() == "Da" ? true : false;
                            }
                        }

                        assessmentQuestionTmp.Answer = answered;
                        assessmentsQuestionsView.Add(assessmentQuestionTmp);
                    }

                    assessmentsGroupTmp.AssessmentQuestions = assessmentsQuestionsView;

                    AssessmentsGroups.Add(assessmentsGroupTmp);
                }
                AssessmentsTypeView.AssessmentGroups = AssessmentsGroups;

                tmpAssessmentsTypes.Add(AssessmentsTypeView);
            }

            return tmpAssessmentsTypes;
        }
    }
}
