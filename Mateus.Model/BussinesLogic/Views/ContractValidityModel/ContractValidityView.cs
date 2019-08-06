using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.ContractValidityModel
{
    public class ContractValidityView
    {
        public int? ContractValidityPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        public bool? Deleted { get; set; }

        public void ConvertFrom(ContractValidity contractValidity, ContractValidityView contractValidityView) 
        {
            contractValidityView.ContractValidityPK = contractValidity.ContractValidityPK;
            contractValidityView.Name = contractValidity.Name;
            contractValidityView.Deleted = contractValidity.Deleted;
        }

        public void ConvertTo(ContractValidityView contractValidityView, ContractValidity contractValidity) 
        {
            contractValidity.Name = contractValidityView.Name;
        }
    }
}
