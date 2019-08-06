using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityBranchModel
{
    public class LegalEntityBranchView
    {
        public int LegalEntityBranchPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Tvrtka je obavezna.")]
        public int? LegalEntityFK { get; set; }

        [Required(ErrorMessage = "Država je obavezna.")]
        public int? CountryFK { get; set; }

        [Required(ErrorMessage = "Županija je obavezna.")]
        public int? CountyFK { get; set; }

        [Required(ErrorMessage = "Grad/općina je obavezan.")]
        public int? CityCommunityFK { get; set; }

        [Required(ErrorMessage = "Poštanski broj je obavezan.")]
        public int? PostalOfficeFK { get; set; }

        [Required(ErrorMessage = "Naselje je obavezno.")]
        public int? PlaceFK { get; set; }

        [Required(ErrorMessage = "Ulica i broj su obavezni.")]
        public string StreetName { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string EMail { get; set; }

        [Required(ErrorMessage = "Datum rješenja je obavezan.")]
        public DateTime? ChangeDate { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
        public IEnumerable<SelectListItem> Counties { get; set; }
        public IEnumerable<SelectListItem> CitiesCommunities { get; set; }
        public IEnumerable<SelectListItem> PostalOffices { get; set; }
        public IEnumerable<SelectListItem> Places { get; set; }
        public IEnumerable<SelectListItem> LegalEntities { get; set; }

        public string LegalEntityName { get; set; }

        public void ConvertFrom(LegalEntityBranch legalEntityBranch, LegalEntityBranchView legalEntityBranchView, ObjectContext db) 
        {
            legalEntityBranchView.LegalEntityBranchPK   = legalEntityBranch.LegalEntityBranchPK;
            legalEntityBranchView.Name                  = legalEntityBranch.Name;
            legalEntityBranchView.LegalEntityFK         = legalEntityBranch.LegalEntityFK;
            legalEntityBranchView.CountryFK             = legalEntityBranch.CountryFK;
            legalEntityBranchView.CountyFK              = legalEntityBranch.CountyFK;
            legalEntityBranchView.CityCommunityFK       = legalEntityBranch.CityCommunityFK;
            legalEntityBranchView.PostalOfficeFK        = legalEntityBranch.PostalOfficeFK;
            legalEntityBranchView.PlaceFK               = legalEntityBranch.PlaceFK;
            legalEntityBranchView.StreetName            = legalEntityBranch.StreetName;
            legalEntityBranchView.Phone                 = legalEntityBranch.Phone;
            legalEntityBranchView.Fax                   = legalEntityBranch.Fax;
            legalEntityBranchView.Mobile                = legalEntityBranch.Mobile;
            legalEntityBranchView.EMail                 = legalEntityBranch.EMail;
            legalEntityBranchView.Deleted               = legalEntityBranch.Deleted;

            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityBranchView.LegalEntityFK);
            legalEntityBranchView.LegalEntityName = legalEntity.Name + " (" + legalEntity.OIB + ")";
        }

        public void ConvertTo(LegalEntityBranchView legalEntityBranchView, LegalEntityBranch legalEntityBranch) 
        {
            legalEntityBranch.LegalEntityBranchPK   = legalEntityBranchView.LegalEntityBranchPK;
            legalEntityBranch.Name                  = legalEntityBranchView.Name;
            legalEntityBranch.LegalEntityFK         = legalEntityBranchView.LegalEntityFK;
            legalEntityBranch.CountryFK             = legalEntityBranchView.CountryFK;
            legalEntityBranch.CountyFK              = legalEntityBranchView.CountyFK;
            legalEntityBranch.CityCommunityFK       = legalEntityBranchView.CityCommunityFK;
            legalEntityBranch.PostalOfficeFK        = legalEntityBranchView.PostalOfficeFK;
            legalEntityBranch.PlaceFK               = legalEntityBranchView.PlaceFK;
            legalEntityBranch.StreetName            = legalEntityBranchView.StreetName;
            legalEntityBranch.Phone                 = legalEntityBranchView.Phone;
            legalEntityBranch.Fax                   = legalEntityBranchView.Fax;
            legalEntityBranch.Mobile                = legalEntityBranchView.Mobile;
            legalEntityBranch.EMail                 = legalEntityBranchView.EMail;
            legalEntityBranch.ChangeDate          = legalEntityBranchView.ChangeDate;
            legalEntityBranch.Deleted               = legalEntityBranchView.Deleted;
        }

        public void BindDDLs(LegalEntityBranchView legalEntityBranchView, ObjectContext db) 
        {
            //countries ddl
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            legalEntityBranchView.Countries = new SelectList(countriesRepository.GetValid().ToList(), "CountryPK", "Name");

            //counties ddl
            ICountiesRepository countiesRepository = new CountiesRepository(db);
            legalEntityBranchView.Counties = new SelectList(countiesRepository.GetCountiesByCountry((int)legalEntityBranchView.CountryFK).ToList(), "CountyPK", "Name");

            //citiesCommunities dll
            if (legalEntityBranchView.CountyFK != null)
            {
                ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);
                legalEntityBranchView.CitiesCommunities = new SelectList(citiesCommunitiesRepository.GetCitiesCommunitiesByCounty(Convert.ToInt32(legalEntityBranchView.CountyFK)), "CityCommunityPK", "Name", legalEntityBranchView.CityCommunityFK);
            }
            else
            {
                legalEntityBranchView.CitiesCommunities = new SelectList(new List<CityCommunity>(), "CityCommunityPK", "Name");
            }

            //postal offices dll
            if (legalEntityBranchView.CountyFK != null)
            {
                IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
                legalEntityBranchView.PostalOffices = new SelectList(postalOfficesRepository.GetValidByCounty(Convert.ToInt32(legalEntityBranchView.CountyFK)), "PostalOfficePK", "Name", legalEntityBranchView.PostalOfficeFK);
            }
            else
            {
                legalEntityBranchView.PostalOffices = new SelectList(new List<PostalOffice>(), "PostalOfficePK", "Name");
            }

            //places dll
            if (legalEntityBranchView.PostalOfficeFK != null)
            {
                IPlacesRepository placesRepository = new PlacesRepository(db);
                legalEntityBranchView.Places = new SelectList(placesRepository.GetPlacesByPostalOffice(Convert.ToInt32(legalEntityBranchView.PostalOfficeFK)), "PlacePK", "Name", legalEntityBranchView.PlaceFK);
            }
            else
            {
                legalEntityBranchView.Places = new SelectList(new List<Place>(), "PlacePK", "Name");
            }
        }

        public static IQueryable<LegalEntityBranchView> GetLegalEntityBranchView(IQueryable<LegalEntityBranch> legalEntityBranchTable, IQueryable<LegalEntity> legalEntityTable) 
        {
            IQueryable<LegalEntityBranchView> legalEntityBranchViewList = (from t1 in legalEntityBranchTable
                                       join t2 in legalEntityTable on t1.LegalEntityFK equals t2.LegalEntityPK

                                       select new LegalEntityBranchView
                                       {
                                            LegalEntityBranchPK = t1.LegalEntityBranchPK,
                                            Name                = t1.Name,
                                            LegalEntityName     = t2.Name,
                                            LegalEntityFK       = t1.LegalEntityFK
                                       }).AsQueryable<LegalEntityBranchView>();

            return legalEntityBranchViewList;
        }
    }
}
