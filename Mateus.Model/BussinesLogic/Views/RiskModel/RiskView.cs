using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.RiskModel
{
    public class RiskView
    {
        public int RiskPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(Risk risk, RiskView riskView) 
        {
            riskView.RiskPK = risk.RiskPK;
            riskView.Name = risk.Name;
            riskView.Deleted = risk.Deleted;
        }

        public void ConvertTo(RiskView riskView, Risk risk) 
        {
            risk.Name = riskView.Name;
        }
    }
}
