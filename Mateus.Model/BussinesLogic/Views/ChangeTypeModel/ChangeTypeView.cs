using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.ChangeTypeModel
{
    public class ChangeTypeView
    {
        public int ChangeTypePK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(ChangeType changeType, ChangeTypeView changeTypeView) 
        {
            changeTypeView.ChangeTypePK = changeType.ChangeTypePK;
            changeTypeView.Name = changeType.Name;
            changeTypeView.Deleted = changeType.Deleted;
        }

        public void ConvertTo(ChangeTypeView changeTypeView, ChangeType changeType) 
        {
            changeType.ChangeTypePK = changeTypeView.ChangeTypePK;
            changeType.Name = changeTypeView.Name;
            changeType.Deleted = changeTypeView.Deleted;
        }
    }
}
