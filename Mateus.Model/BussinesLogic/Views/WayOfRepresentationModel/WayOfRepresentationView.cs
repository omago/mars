using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.WayOfRepresentationModel
{
    public class WayOfRepresentationView
    {
        public int WayOfRepresentationPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(WayOfRepresentation wayOfRepresentation, WayOfRepresentationView wayOfRepresentationView) 
        {
            wayOfRepresentationView.WayOfRepresentationPK = wayOfRepresentation.WayOfRepresentationPK;
            wayOfRepresentationView.Name = wayOfRepresentation.Name;
            wayOfRepresentationView.Deleted = wayOfRepresentation.Deleted;
        }

        public void ConvertTo(WayOfRepresentationView wayOfRepresentationView, WayOfRepresentation wayOfRepresentation) 
        {
            wayOfRepresentation.Name = wayOfRepresentationView.Name;
        }
    }
}
