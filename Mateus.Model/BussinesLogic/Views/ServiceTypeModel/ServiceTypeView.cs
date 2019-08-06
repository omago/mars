using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.ServiceTypeModel
{
    public class ServiceTypeView
    {
        public int ServiceTypePK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(ServiceType serviceType, ServiceTypeView serviceTypeView) 
        {
            serviceTypeView.ServiceTypePK = serviceType.ServiceTypePK;
            serviceTypeView.Name = serviceType.Name;
            serviceTypeView.Deleted = serviceType.Deleted;
        }

        public void ConvertTo(ServiceTypeView serviceTypeView, ServiceType serviceType) 
        {
            serviceType.Name = serviceTypeView.Name;
        }
    }
}
