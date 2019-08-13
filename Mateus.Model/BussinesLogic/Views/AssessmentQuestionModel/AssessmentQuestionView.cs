using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;
using PITFramework.Support;

namespace Mateus.Model.BussinesLogic.Views.AssessmentQuestionModel
{
    public class AssessmentQuestionView
    {
        public int AssessmentQuestionPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
  
        [Required(ErrorMessage = "Grupa procjene je obavezna.")]
        public int? AssessmentGroupFK { get; set; }
  
        [Required(ErrorMessage = "Tip procjene je obavezan.")]
        public int? AssessmentTypeFK { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> AssessmentGroups { get; set; }
        public IEnumerable<SelectListItem> AssessmentTypes { get; set; }

        public string AssessmentGroupName { get; set; }
        public string AssessmentTypeName { get; set; }

        public int? AssessmentQuestionCount { get; set; }

        public void ConvertFrom(AssessmentQuestion assessmentQuestion, AssessmentQuestionView assessmentQuestionView, ObjectContext db) 
        {
            assessmentQuestionView.AssessmentQuestionPK = assessmentQuestion.AssessmentQuestionPK;
            assessmentQuestionView.Name = assessmentQuestion.Name;
            assessmentQuestionView.AssessmentGroupFK = assessmentQuestion.AssessmentGroupFK;
            assessmentQuestionView.Deleted = assessmentQuestion.Deleted;

            //get assement type fk
            IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
            assessmentQuestionView.AssessmentTypeFK = assessmentGroupsRepository.GetAssessmentGroupByPK((int)assessmentQuestionView.AssessmentGroupFK).AssessmentTypeFK;
        }

        public void ConvertTo(AssessmentQuestionView assessmentQuestionView, AssessmentQuestion assessmentQuestion) 
        {
            assessmentQuestion.AssessmentQuestionPK = assessmentQuestionView.AssessmentQuestionPK;
            assessmentQuestion.Name = assessmentQuestionView.Name;
            assessmentQuestion.AssessmentGroupFK = assessmentQuestionView.AssessmentGroupFK;
            assessmentQuestion.Deleted = assessmentQuestionView.Deleted;
        }

        public void BindDDLs(AssessmentQuestionView assessmentQuestionView, ObjectContext db) 
        {
            IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
            assessmentQuestionView.AssessmentTypes = new SelectList(assessmentTypesRepository.GetValid().OrderBy("Name ASC").ToList(), "AssessmentTypePK", "Name");

            //assement question ddl
            if (assessmentQuestionView.AssessmentTypeFK != null)
            {
                IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
                assessmentQuestionView.AssessmentGroups = new SelectList(assessmentGroupsRepository.GetAssessmentGroupsByType(Convert.ToInt32((int)assessmentQuestionView.AssessmentTypeFK)), "AssessmentGroupPK", "Name");
            }
            else
            {
                assessmentQuestionView.AssessmentGroups = new SelectList(new List<AssessmentGroup>(), "AssessmentGroupPK", "Name");
            }
        }

        public static IQueryable<AssessmentQuestionView> GetAssessmentQuestionView(IQueryable<AssessmentQuestion> assessmentQuestionTable, IQueryable<AssessmentGroup> assessmentGroupTable, IQueryable<AssessmentType> assessmentTypeTable) 
        {
            IQueryable<AssessmentQuestionView> assessmentQuestionViewList = (from t1 in assessmentQuestionTable
                                        join t2 in assessmentGroupTable on t1.AssessmentGroupFK equals t2.AssessmentGroupPK
                                        join t3 in assessmentTypeTable on t2.AssessmentTypeFK equals t3.AssessmentTypePK

                                        select new AssessmentQuestionView
                                        {
                                            AssessmentQuestionPK    = t1.AssessmentQuestionPK,
                                            Name                    = t1.Name,
                                            AssessmentGroupName     = t2.Name,
                                            AssessmentGroupFK       = t1.AssessmentGroupFK,
                                            AssessmentTypeName      = t3.Name,
                                            AssessmentTypeFK        = t2.AssessmentTypeFK,
                                        }).AsQueryable<AssessmentQuestionView>();

            return assessmentQuestionViewList;
        }
    }
}
