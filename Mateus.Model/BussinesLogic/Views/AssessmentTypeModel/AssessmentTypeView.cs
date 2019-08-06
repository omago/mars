using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.AssessmentTypeModel
{
    public class AssessmentTypeView
    {
        public int AssessmentTypePK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public int? AssessmentGroupCount { get; set; }

        public void ConvertFrom(AssessmentType assessmentType, AssessmentTypeView assessmentTypeView) 
        {
            assessmentTypeView.AssessmentTypePK = assessmentType.AssessmentTypePK;
            assessmentTypeView.Name = assessmentType.Name;
            assessmentTypeView.Deleted = assessmentType.Deleted;
        }

        public void ConvertTo(AssessmentTypeView assessmentTypeView, AssessmentType assessmentType) 
        {
            assessmentType.Name = assessmentTypeView.Name;
        }

        public static IQueryable<AssessmentTypeView> GetAssessmentTypeListView(IQueryable<AssessmentType> assessmentTypeTable, IQueryable<AssessmentGroup> assessmentGroupTable) 
        {
            IQueryable<AssessmentTypeView> assessmentTypeViewList = (from t1 in assessmentTypeTable

                                       select new AssessmentTypeView
                                       {
                                            AssessmentTypePK        = t1.AssessmentTypePK,
                                            Name                    = t1.Name,
                                            AssessmentGroupCount    = assessmentGroupTable.Where(b => b.AssessmentTypeFK == t1.AssessmentTypePK).Count(),
                                       }).AsQueryable<AssessmentTypeView>();

            return assessmentTypeViewList;
        }
    }
}
