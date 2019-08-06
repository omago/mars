using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;

namespace Mateus.Model.BussinesLogic.Views.AssessmentGroupModel
{
    public class AssessmentGroupView
    {
        public int AssessmentGroupPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
  
        [Required(ErrorMessage = "Tip procjene je obavezan.")]
        public int? AssessmentTypeFK { get; set; }
        
        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> AssessmentTypes { get; set; }

        public string AssessmentTypeName { get; set; }
        public int? AssessmentQuestionCount { get; set; }

        public void ConvertFrom(AssessmentGroup assessmentGroup, AssessmentGroupView assessmentGroupView) 
        {
            assessmentGroupView.AssessmentGroupPK = assessmentGroup.AssessmentGroupPK;
            assessmentGroupView.Name = assessmentGroup.Name;
            assessmentGroupView.AssessmentTypeFK = assessmentGroup.AssessmentTypeFK;
            assessmentGroupView.Deleted = assessmentGroup.Deleted;
        }

        public void ConvertTo(AssessmentGroupView assessmentGroupView, AssessmentGroup assessmentGroup) 
        {
            assessmentGroup.AssessmentGroupPK = assessmentGroupView.AssessmentGroupPK;
            assessmentGroup.Name = assessmentGroupView.Name;
            assessmentGroup.AssessmentTypeFK = assessmentGroupView.AssessmentTypeFK;
            assessmentGroup.Deleted = assessmentGroupView.Deleted;
        }

        public void BindDDLs(AssessmentGroupView assessmentGroupView, ObjectContext db) 
        {
            IAssessmentTypesRepository assessmentTypesRepository = new AssessmentTypesRepository(db);
            assessmentGroupView.AssessmentTypes = new SelectList(assessmentTypesRepository.GetValid().ToList(), "AssessmentTypePK", "Name");
        }

        public static IQueryable<AssessmentGroupView> GetAssessmentGroupView(IQueryable<AssessmentGroup> assessmentGroupTable, IQueryable<AssessmentType> assessmentTypeTable, IQueryable<AssessmentQuestion> assessmentQuestionTable) 
        {
            IQueryable<AssessmentGroupView> assessmentGroupViewList = (from t1 in assessmentGroupTable
                                        join t2 in assessmentTypeTable on t1.AssessmentTypeFK equals t2.AssessmentTypePK

                                        select new AssessmentGroupView
                                        {
                                            AssessmentGroupPK       = t1.AssessmentGroupPK,
                                            Name                    = t1.Name,
                                            AssessmentTypeName      = t2.Name,
                                            AssessmentTypeFK        = t1.AssessmentTypeFK,
                                            AssessmentQuestionCount = assessmentQuestionTable.Where(b => b.AssessmentGroupFK == t1.AssessmentGroupPK).Count(),
                                        }).AsQueryable<AssessmentGroupView>();

            return assessmentGroupViewList;
        }
    }
}
