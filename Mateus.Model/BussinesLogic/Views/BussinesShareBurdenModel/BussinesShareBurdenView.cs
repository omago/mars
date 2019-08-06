using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.BussinesShareBurdenModel
{
    public class BussinesShareBurdenView
    {
        public int BussinesShareBurdenPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(BussinesShareBurden bussinesShareBurden, BussinesShareBurdenView bussinesShareBurdenView) 
        {
            bussinesShareBurdenView.BussinesShareBurdenPK = bussinesShareBurden.BussinesShareBurdenPK;
            bussinesShareBurdenView.Name = bussinesShareBurden.Name;
            bussinesShareBurdenView.Deleted = bussinesShareBurden.Deleted;
        }

        public void ConvertTo(BussinesShareBurdenView bussinesShareBurdenView, BussinesShareBurden bussinesShareBurden) 
        {
            bussinesShareBurden.BussinesShareBurdenPK = bussinesShareBurdenView.BussinesShareBurdenPK;
            bussinesShareBurden.Name = bussinesShareBurdenView.Name;
            bussinesShareBurden.Deleted = bussinesShareBurdenView.Deleted;
        }
    }
}
