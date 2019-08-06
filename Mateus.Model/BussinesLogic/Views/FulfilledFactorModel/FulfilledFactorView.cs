using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.FulfilledFactorModel
{
    public class FulfilledFactorView
    {
        public int FulfilledFactorPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(FulfilledFactor fulfilledFactor, FulfilledFactorView fulfilledFactorView) 
        {
            fulfilledFactorView.FulfilledFactorPK = fulfilledFactor.FulfilledFactorPK;
            fulfilledFactorView.Name = fulfilledFactor.Name;
            fulfilledFactorView.Deleted = fulfilledFactor.Deleted;
        }

        public void ConvertTo(FulfilledFactorView fulfilledFactorView, FulfilledFactor fulfilledFactor) 
        {
            fulfilledFactor.FulfilledFactorPK = fulfilledFactorView.FulfilledFactorPK;
            fulfilledFactor.Name = fulfilledFactorView.Name;
            fulfilledFactor.Deleted = fulfilledFactorView.Deleted;
        }
    }
}