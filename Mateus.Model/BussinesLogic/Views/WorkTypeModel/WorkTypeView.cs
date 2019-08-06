using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.WorkTypeModel
{
    public class WorkTypeView
    {
        public int WorkTypePK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(WorkType workType, WorkTypeView workTypeView) 
        {
            workTypeView.WorkTypePK = workType.WorkTypePK;
            workTypeView.Name = workType.Name;
            workTypeView.Deleted = workType.Deleted;
        }

        public void ConvertTo(WorkTypeView workTypeView, WorkType workType) 
        {
            workType.Name = workTypeView.Name;
        }
    }
}
