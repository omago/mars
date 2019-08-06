using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;

namespace Mateus.Model.BussinesLogic.Views.CityCommunityModel
{
    public class CityCommunityView
    {
        public int CityCommunityPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
  
        [Required(ErrorMessage = "Država je obavezna.")]
        public int? CountryFK { get; set; }
  
        [Required(ErrorMessage = "Šifra je obavezna.")]
        public int? Code { get; set; }

        [Required(ErrorMessage = "Županija je obavezna.")]
        public int? CountyFK { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Counties { get; set; }
        public IEnumerable<SelectListItem> Countries { get; set; }

        public string CountyName { get; set; }
        public string CountryName { get; set; }

        public void ConvertFrom(CityCommunity cityCommunity, CityCommunityView cityCommunityView, ObjectContext db) 
        {
            cityCommunityView.CityCommunityPK = cityCommunity.CityCommunityPK;
            cityCommunityView.Name = cityCommunity.Name;
            cityCommunityView.Code = cityCommunity.Code;
            cityCommunityView.CountyFK = cityCommunity.CountyFK;

            //get country id
            ICountiesRepository countiesRepository = new CountiesRepository(db);
            cityCommunityView.CountryFK = countiesRepository.GetCountyByPK((int)cityCommunityView.CountyFK).CountryFK;
        }

        public void ConvertTo(CityCommunityView cityCommunityView, CityCommunity cityCommunity) 
        {
            cityCommunity.CityCommunityPK = cityCommunityView.CityCommunityPK;
            cityCommunity.Name = cityCommunityView.Name;
            cityCommunity.Code = cityCommunityView.Code;
            cityCommunity.CountyFK = cityCommunityView.CountyFK;
        }

        public void BindDDLs(CityCommunityView cityCommunityView, ObjectContext db) 
        {
            //countries ddl
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            cityCommunityView.Countries = new SelectList(countriesRepository.GetValid().ToList(), "CountryPK", "Name");    

            //counties ddl
            if (cityCommunityView.CountryFK != null)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                cityCommunityView.Counties = new SelectList(countiesRepository.GetCountiesByCountry(Convert.ToInt32((int)cityCommunityView.CountryFK)), "CountyPK", "Name");
            }
            else
            {
                cityCommunityView.Counties = new SelectList(new List<County>(), "CountyPK", "Name");
            }
        }

        public static IQueryable<CityCommunityView> GetCityCommunityView(IQueryable<CityCommunity> cityCommunityTable, IQueryable<County> countyTable, IQueryable<Country> countryTable) 
        {
            IQueryable<CityCommunityView> cityCommunityViewList = (from t1 in cityCommunityTable
                                       join t2 in countyTable on t1.CountyFK equals t2.CountyPK
                                       join t3 in countryTable on t2.CountryFK equals t3.CountryPK

                                       select new CityCommunityView
                                       {
                                            CityCommunityPK = t1.CityCommunityPK,
                                            Name            = t1.Name,
                                            Code            = t1.Code,
                                            CountyName      = t2.Name,
                                            CountryName     = t3.Name,
                                       }).AsQueryable<CityCommunityView>();

            return cityCommunityViewList;
        }
    }
}
