using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;
using System.Data.Objects.SqlClient;

namespace Mateus.Model.BussinesLogic.Views.PlaceModel
{
    public class PlaceView
    {
        public int PlacePK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Država je obavezna.")]
        public int? CountryFK { get; set; }
        
        [Required(ErrorMessage = "Županija je obavezna.")]
        public int? CountyFK { get; set; }
        
        [Required(ErrorMessage = "Poštanski ured je obavezan.")]
        public int? PostalOfficeFK { get; set; }

        [Required(ErrorMessage = "Redni broj je obavezan.")]
        public int? Ordinal { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Counties { get; set; }
        public IEnumerable<SelectListItem> Countries { get; set; }
        public IEnumerable<SelectListItem> PostalOffices { get; set; }

        public string PostalOfficeName { get; set; }
        public int? PostalOfficeNumber { get; set; }
        public string CountyName { get; set; }
        public string CountryName { get; set; }

        public void ConvertFrom(Place place, PlaceView placeView, ObjectContext db)
        {
            placeView.PlacePK = place.PlacePK;
            placeView.Name = place.Name;
            placeView.Ordinal = place.Ordinal;
            placeView.PostalOfficeFK = place.PostalOfficeFK;
            placeView.Deleted = place.Deleted;

            //get county
            IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
            placeView.CountyFK = (int)postalOfficesRepository.GetPostalOfficeByPK((int)placeView.PostalOfficeFK).CountyFK;

            //get country
            ICountiesRepository countiesRepository = new CountiesRepository(db);
            placeView.CountryFK = countiesRepository.GetCountyByPK((int)placeView.CountyFK).CountryFK;
        }

        public void ConvertTo(PlaceView placeView, Place place)
        {
            place.PlacePK = placeView.PlacePK;
            place.Name = placeView.Name;
            place.Ordinal = placeView.Ordinal;
            place.PostalOfficeFK = placeView.PostalOfficeFK;
            place.Deleted = placeView.Deleted;
        }

        public void BindDDLs(PlaceView placeView, ObjectContext db) 
        {
            //countries ddl
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            placeView.Countries = new SelectList(countriesRepository.GetValid().ToList(), "CountryPK", "Name");

            //counties ddl
            if (placeView.CountryFK != null)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                placeView.Counties = new SelectList(countiesRepository.GetCountiesByCountry(Convert.ToInt32(placeView.CountryFK)), "CountyPK", "Name");
            }
            else
            {
                placeView.Counties = new SelectList(new List<County>(), "CountyPK", "Name");
            }

            //postal offices ddl
            if (placeView.CountyFK != null)
            {
                IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
                var postalOffices = postalOfficesRepository.GetValidByCounty(Convert.ToInt32(placeView.CountyFK)).OrderBy(c => c.Name);

                placeView.PostalOffices = new SelectList(postalOffices.Select(c => new { value = c.PostalOfficePK, text = c.Name + " (" + SqlFunctions.StringConvert((double)c.Number).Trim() + ")" }), "value", "text");
            }
            else
            {
                placeView.PostalOffices = new SelectList(new List<PostalOffice>(), "PostalOfficePK", "Name");
            }    
        }

        public static IQueryable<PlaceView> GetPlaceView(IQueryable<Place> placeTable, IQueryable<PostalOffice> postalOfficeTable, IQueryable<County> countyTable, IQueryable<Country> countryTable) 
        {
            IQueryable<PlaceView> placeViewist = (from t1 in placeTable
                                       join t2 in postalOfficeTable on t1.PostalOfficeFK equals t2.PostalOfficePK
                                       join t3 in countyTable on t2.CountyFK equals t3.CountyPK
                                       join t4 in countryTable on t3.CountryFK equals t4.CountryPK

                                       select new PlaceView
                                       {
                                            PlacePK             = t1.PlacePK,
                                            Name                = t1.Name,
                                            Ordinal             = t1.Ordinal,
                                            PostalOfficeName    = t2.Name,
                                            PostalOfficeNumber  = t2.Number,
                                            CountyName          = t3.Name,
                                            CountryName         = t4.Name,
                                       }).AsQueryable<PlaceView>();

            return placeViewist;
        }
    }
}
