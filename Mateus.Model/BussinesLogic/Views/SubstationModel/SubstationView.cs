using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;

namespace Mateus.Model.BussinesLogic.Views.SubstationModel
{
    public class SubstationView
    {
        public int SubstationPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Regionalni ured je obavezan.")]
        public int? RegionalOfficeFK { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> RegionalOffices { get; set; }

        public string RegionalOfficeName { get; set; }

        public void ConvertFrom(Substation substation, SubstationView substationView) 
        {
            substationView.SubstationPK = substation.SubstationPK;
            substationView.Name = substation.Name;
            substationView.RegionalOfficeFK = substation.RegionalOfficeFK;
            substationView.Deleted = substation.Deleted;
        }

        public void ConvertTo(SubstationView substationView, Substation substation) 
        {
            substation.SubstationPK = substationView.SubstationPK;
            substation.Name = substationView.Name;
            substation.RegionalOfficeFK = substationView.RegionalOfficeFK;
        }

        public void BindDDLs(SubstationView substationView, ObjectContext db) 
        {
            IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);
            substationView.RegionalOffices = new SelectList(regionalOfficesRepository.GetValid().ToList(), "RegionalOfficePK", "Name");
        }

        public static IQueryable<SubstationView> GetSubstationView(IQueryable<Substation> substationTable, IQueryable<RegionalOffice> regionalOfficeTable) 
        {
            IQueryable<SubstationView> substationViewList = (from t1 in substationTable
                                       join t2 in regionalOfficeTable on t1.RegionalOfficeFK equals t2.RegionalOfficePK

                                       select new SubstationView
                                       {
                                            SubstationPK        = t1.SubstationPK,
                                            Name                = t1.Name,
                                            RegionalOfficeName  = t2.Name,
                                       }).AsQueryable<SubstationView>();

            return substationViewList;
        }
    }
}
