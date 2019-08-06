using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.AdditionalFactorModel
{
    public class AdditionalFactorView
    {
        public int AdditionalFactorPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(AdditionalFactor additionalFactor, AdditionalFactorView additionalFactorView) 
        {
            additionalFactorView.AdditionalFactorPK = additionalFactor.AdditionalFactorPK;
            additionalFactorView.Name = additionalFactor.Name;
            additionalFactorView.Deleted = additionalFactor.Deleted;
        }

        public void ConvertTo(AdditionalFactorView additionalFactorView, AdditionalFactor additionalFactor) 
        {
            additionalFactor.AdditionalFactorPK = additionalFactorView.AdditionalFactorPK;
            additionalFactor.Name = additionalFactorView.Name;
            additionalFactor.Deleted = additionalFactorView.Deleted;
        }
    }
}
