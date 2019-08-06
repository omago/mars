using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.CommercialCourtModel
{
    public class CommercialCourtView
    {
        public int CommercialCourtPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        public bool? Deleted { get; set; }

        public void ConvertFrom(CommercialCourt commercialCourt, CommercialCourtView commercialCourtView) 
        {
            commercialCourtView.CommercialCourtPK = commercialCourt.CommercialCourtPK;
            commercialCourtView.Name = commercialCourt.Name;
            commercialCourtView.Deleted = commercialCourt.Deleted;
        }

        public void ConvertTo(CommercialCourtView commercialCourtView, CommercialCourt commercialCourt) 
        {
            commercialCourt.CommercialCourtPK = commercialCourtView.CommercialCourtPK;
            commercialCourt.Name = commercialCourtView.Name;
        }
    }
}
