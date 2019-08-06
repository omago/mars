using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.FormModel
{
    public class FormView
    {
        public int FormPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        public bool? Deleted { get; set; }

        public void ConvertFrom(Form form, FormView formView) 
        {
            formView.FormPK = form.FormPK;
            formView.Name = form.Name;
            formView.Deleted = form.Deleted;
        }

        public void ConvertTo(FormView formView, Form form) 
        {
            form.Name = formView.Name;
        }
    }
}
