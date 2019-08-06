using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.RegionalOfficeModel
{
    public class RegionalOfficeView
    {
        public int RegionalOfficePK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        public bool? Deleted { get; set; }

        public void ConvertFrom(RegionalOffice regionalOffice, RegionalOfficeView regionalOfficeView) 
        {
            regionalOfficeView.RegionalOfficePK = regionalOffice.RegionalOfficePK;
            regionalOfficeView.Name = regionalOffice.Name;
            regionalOfficeView.Deleted = regionalOffice.Deleted;
        }

        public void ConvertTo(RegionalOfficeView regionalOfficeView, RegionalOffice regionalOffice) 
        {
            regionalOffice.RegionalOfficePK = regionalOfficeView.RegionalOfficePK;
            regionalOffice.Name = regionalOfficeView.Name;
        }
    }
}
