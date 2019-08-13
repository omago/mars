using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;
using PITFramework.Support;

namespace Mateus.Model.BussinesLogic.Views.CountyModel
{
    public class CountyView
    {
        public int CountyPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
  
        [Required(ErrorMessage = "Država je obavezna.")]
        public int? CountryFK { get; set; }
        
        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public string CountryName { get; set; }

        public void ConvertFrom(County county, CountyView countyView) 
        {
            countyView.CountyPK = county.CountyPK;
            countyView.Name = county.Name;
            countyView.CountryFK = county.CountryFK;
            countyView.Deleted = county.Deleted;
        }

        public void ConvertTo(CountyView countyView, County county) 
        {
            county.CountyPK = countyView.CountyPK;
            county.Name = countyView.Name;
            county.CountryFK = countyView.CountryFK;
            county.Deleted = countyView.Deleted;
        }

        public void BindDDLs(CountyView countyView, ObjectContext db) 
        {
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            countyView.Countries = new SelectList(countriesRepository.GetValid().OrderBy("Name ASC").ToList(), "CountryPK", "Name");
        }

        public static IQueryable<CountyView> GetCountyView(IQueryable<County> countyTable, IQueryable<Country> countryTable) 
        {
            IQueryable<CountyView> countyViewList = (from t1 in countyTable
                                       join t2 in countryTable on t1.CountryFK equals t2.CountryPK

                                       select new CountyView
                                       {
                                            CountyPK        = t1.CountyPK,
                                            Name            = t1.Name,
                                            CountryName     = t2.Name,
                                       }).AsQueryable<CountyView>();

            return countyViewList;
        }
    }
}
