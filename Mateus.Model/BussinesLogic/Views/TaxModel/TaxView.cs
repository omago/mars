using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.TaxModel
{
    public class TaxView
    {
        public int TaxPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(Tax tax, TaxView taxView) 
        {
            taxView.TaxPK = tax.TaxPK;
            taxView.Name = tax.Name;
            taxView.Deleted = tax.Deleted;
        }

        public void ConvertTo(TaxView taxView, Tax tax) 
        {
            tax.Name = taxView.Name;
        }
    }
}
