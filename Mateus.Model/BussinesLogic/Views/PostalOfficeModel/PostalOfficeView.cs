using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;

namespace Mateus.Model.BussinesLogic.Views.PostalOfficeModel
{
    public class PostalOfficeView
    {
        public int PostalOfficePK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Država je obavezna.")]
        public int? CountryFK { get; set; }
        
        [Required(ErrorMessage = "Županija je obavezna.")]
        public int? CountyFK { get; set; }

        [Required(ErrorMessage = "Poštanski broj je obavezan.")]
        public int? Number { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Counties { get; set; }
        public IEnumerable<SelectListItem> Countries { get; set; }

        public string CountyName { get; set; }
        public string CountryName { get; set; }

        public void ConvertFrom(PostalOffice postalOffice, PostalOfficeView postalOfficeView, ObjectContext db) 
        {
            postalOfficeView.PostalOfficePK = postalOffice.PostalOfficePK;
            postalOfficeView.Name = postalOffice.Name;
            postalOfficeView.CountyFK = postalOffice.CountyFK;
            postalOfficeView.Number = postalOffice.Number;
            postalOfficeView.Deleted = postalOffice.Deleted;

            //get country id
            ICountiesRepository countiesRepository = new CountiesRepository(db);
            postalOfficeView.CountryFK = countiesRepository.GetCountyByPK((int)postalOfficeView.CountyFK).CountryFK;
        }

        public void ConvertTo(PostalOfficeView postalOfficeView, PostalOffice postalOffice) 
        {
            postalOffice.PostalOfficePK = postalOfficeView.PostalOfficePK;
            postalOffice.Name = postalOfficeView.Name;
            postalOffice.CountyFK = postalOfficeView.CountyFK;
            postalOffice.Number = postalOfficeView.Number;
        }

        public void BindDDLs(PostalOfficeView postalOfficeView, ObjectContext db) 
        {
            //countries ddl
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            postalOfficeView.Countries = new SelectList(countriesRepository.GetValid().ToList(), "CountryPK", "Name");    

            //counties ddl
            if (postalOfficeView.CountryFK != null)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                postalOfficeView.Counties = new SelectList(countiesRepository.GetCountiesByCountry(Convert.ToInt32((int)postalOfficeView.CountryFK)), "CountyPK", "Name");
            }
            else
            {
                postalOfficeView.Counties = new SelectList(new List<County>(), "CountyPK", "Name");
            }
        }

        public static IQueryable<PostalOfficeView> GetPostalOfficeView(IQueryable<PostalOffice> postalOfficeTable, IQueryable<County> countyTable, IQueryable<Country> countryTable) 
        {
            IQueryable<PostalOfficeView> postalOfficeViewList = (from t1 in postalOfficeTable
                                       join t2 in countyTable on t1.CountyFK equals t2.CountyPK
                                       join t3 in countryTable on t2.CountryFK equals t3.CountryPK

                                       select new PostalOfficeView
                                       {
                                            PostalOfficePK  = t1.PostalOfficePK,
                                            Name            = t1.Name,
                                            Number          = t1.Number,
                                            CountyName      = t2.Name,
                                            CountryName     = t3.Name,
                                       }).AsQueryable<PostalOfficeView>();

            return postalOfficeViewList;
        }
    }
}
