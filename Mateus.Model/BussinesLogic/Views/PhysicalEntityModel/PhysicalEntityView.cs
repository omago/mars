using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.BussinesLogic.Support.Validation;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using Mateus.Model.BussinesLogic.Views.GeneratorModel;
using PITFramework.Support;

namespace Mateus.Model.BussinesLogic.Views.PhysicalEntityModel
{
    public class PhysicalEntityView
    {
        public int PhysicalEntityPK { get; set; }

        [Required(ErrorMessage = "Ime je obavezno."), StringLength(256, ErrorMessage = "Ime ne smije biti duže od 256 znakova.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno."), StringLength(256, ErrorMessage = "Prezime ne smije biti duže od 256 znakova.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Spol je obavezan.")]
        public string Gender { get; set; }

        [RequiredIf("Referent == true", ErrorMessage = "E-mail je obavezan."), Email(ErrorMessage = "Email adresa nije ispravna.")]
        public string EMail { get; set; }

        [RequiredIf("Owner == true || LegalRepresentative == true", ErrorMessage = "OIB je obavezan."), StringLength(11, ErrorMessage = "OIB ne smije biti duži od 11 znakova.")]
        public string OIB { get; set; }

        [StringLength(13, ErrorMessage = "JMBG ne smije biti duži od 13 znakova.")]
        public string JMBG { get; set; }

        [RequiredIf("Owner == true || LegalRepresentative == true", ErrorMessage = "Datum rođenja je obavezan.")]
        public DateTime? DateOfBirth { get; set; }

        [RequiredIf("Owner == true || LegalRepresentative == true", ErrorMessage = "Državljanstvo je obavezno.")]
        public int? CitizenshipFK { get; set; }

        [RequiredIf("Owner == true || LegalRepresentative == true", ErrorMessage = "Država rođenja je obavezna.")]
        public int? BirthCountryFK { get; set; }

        [RequiredIf("BirthCountryFK == 81", ErrorMessage = "Županija rođenja je obavezna.")]
        public int? BirthCountyFK { get; set; }

        [RequiredIf("BirthCountryFK == 81", ErrorMessage = "Grad/općina rođenja je obavezan.")]
        public int? BirthCityCommunityFK { get; set; }

        [RequiredIf("BirthCountryFK != 81 && BirthCountryFK != null", ErrorMessage = "Mjesto rođenja je obavezno.")]
        public string BirthPlace { get; set; }

        [RequiredIf("Owner == true || LegalRepresentative == true", ErrorMessage = "Država stanovanja je obavezna.")]
        public int? ResidenceCountryFK { get; set; }

        [RequiredIf("ResidenceCountryFK == 81", ErrorMessage = "Županija stanovanja je obavezna.")]
        public int? ResidenceCountyFK { get; set; }

        [RequiredIf("ResidenceCountryFK == 81", ErrorMessage = "Grad/općina stanovanja je obavezan.")]
        public int? ResidenceCityCommunityFK { get; set; }

        [RequiredIf("ResidenceCountryFK == 81", ErrorMessage = "Poštanski stanovanja je obavezno.")]
        public int? ResidencePostalOfficeFK { get; set; }

        [RequiredIf("ResidenceCountryFK == 81", ErrorMessage = "Naselje stanovanja je obavezno.")]
        public int? ResidencePlaceFK { get; set; }

        [RequiredIf("ResidenceCountryFK != 81 && ResidenceCountryFK != null", ErrorMessage = "Mjesto stanovanja je obavezno.")]
        public string ResidencePlace { get; set; }

        [RequiredIf("Owner == true || LegalRepresentative == true", ErrorMessage = "Ulica stanovanja je obavezna."), StringLength(256, ErrorMessage = "Ulica stanovanja ne smije biti duža od 256 znakova.")]
        public string ResidenceStreetName { get; set; }

        [RequiredIf("CitizenshipFK == 81", ErrorMessage = "Broj osobne iskaznice je obavezan.")]
        public string IdentityCardNumber { get; set; }

        [RequiredIf("CitizenshipFK == 81", ErrorMessage = "Datum izdavanja osobne iskaznice je obavezan.")]
        public DateTime? IdentityCardDateOfIssue { get; set; }

        [RequiredIf("CitizenshipFK == 81", ErrorMessage = "Regionalni ured gdje je osobna iskaznica izdana je obavezan.")]
        public int? IdentityCardRegionalOfficeFK { get; set; }

        [RequiredIf("CitizenshipFK == 81", ErrorMessage = "Datum isteka osobne iskaznice je obavezan.")]
        public DateTime? IdentityCardDateTillValid { get; set; }
        
        [RequiredIf("CitizenshipFK != 81 && CitizenshipFK != null", ErrorMessage = "Broj putovnice je obavezan.")]
        public string PassportNumber { get; set; }

        [RequiredIf("CitizenshipFK != 81 && CitizenshipFK != null", ErrorMessage = "Datum izdavanja putovnice je obavezan.")]
        public DateTime? PassportDateOfIssue { get; set; }

        [RequiredIf("CitizenshipFK != 81 && CitizenshipFK != null", ErrorMessage = "Datum isteka putovnice je obavezan.")]
        public DateTime? PassportDateTillValid { get; set; }

        [RequiredIf("CitizenshipFK != 81 && CitizenshipFK != null", ErrorMessage = "Država izdavanja putovnice je obavezna.")]
        public int? PassportCountryOfIssueFK { get; set; }

        [RequiredIf("CitizenshipFK != 81 && CitizenshipFK != null", ErrorMessage = "Mjesto izdavanja putovnice je obavezno.")]
        public string PassportPlaceOfIssue { get; set; }

        [RequiredIf("Referent == true", ErrorMessage = "Područni ured je obavezan.")]
        public int? ReferentRegionalOfficeFK { get; set; }

        [RequiredIf("Referent == true", ErrorMessage = "Ispostava je obavezna.")]
        public int? ReferentSubstationFK { get; set; }

        [Required(ErrorMessage = "Datum rješenja je obavezan.")]
        public DateTime? ChangeDate { get; set; }

        public bool? LegalRepresentative { get; set; }
        public bool? Owner { get; set; }
        public bool? Referent { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> LegalEntities { get; set; }

        public IEnumerable<SelectListItem> Citizenships { get; set; }
        public IEnumerable<SelectListItem> Genders { get; set; }

        public IEnumerable<SelectListItem> BirthCountries { get; set; }
        public IEnumerable<SelectListItem> BirthCounties { get; set; }
        public IEnumerable<SelectListItem> BirthCitiesCommunities { get; set; }

        public IEnumerable<SelectListItem> ResidenceCountries { get; set; }
        public IEnumerable<SelectListItem> ResidenceCounties { get; set; }
        public IEnumerable<SelectListItem> ResidenceCitiesCommunities { get; set; }
        public IEnumerable<SelectListItem> ResidencePostalOffices { get; set; }
        public IEnumerable<SelectListItem> ResidencePlaces { get; set; }

        public IEnumerable<SelectListItem> IdentityCardRegionalOffices { get; set; }

        public IEnumerable<SelectListItem> PassportCountriesOfIssue { get; set; }

        public IEnumerable<SelectListItem> ReferentRegionalOffices { get; set; }
        public IEnumerable<SelectListItem> ReferentSubstations { get; set; }

        public string Name { get; set; }

        public void ConvertFrom(PhysicalEntity physicalEntity, PhysicalEntityView physicalEntityView) 
        {
            physicalEntityView.PhysicalEntityPK             = physicalEntity.PhysicalEntityPK;

            physicalEntityView.Firstname                    = physicalEntity.Firstname;
            physicalEntityView.Lastname                     = physicalEntity.Lastname;
            physicalEntityView.Gender                       = physicalEntity.Gender;
            physicalEntityView.OIB                          = physicalEntity.OIB;
            physicalEntityView.JMBG                         = physicalEntity.JMBG;
            physicalEntityView.DateOfBirth                  = physicalEntity.DateOfBirth;
            physicalEntityView.CitizenshipFK                = physicalEntity.CitizenshipFK;

            physicalEntityView.BirthCountryFK               = physicalEntity.BirthCountryFK;
            physicalEntityView.BirthCountyFK                = physicalEntity.BirthCountyFK;
            physicalEntityView.BirthCityCommunityFK         = physicalEntity.BirthCityCommunityFK;
            physicalEntityView.BirthPlace                   = physicalEntity.BirthPlace;

            physicalEntityView.ResidenceCountryFK           = physicalEntity.ResidenceCountryFK;
            physicalEntityView.ResidenceCountyFK            = physicalEntity.ResidenceCountyFK;
            physicalEntityView.ResidenceCityCommunityFK     = physicalEntity.ResidenceCityCommunityFK;
            physicalEntityView.ResidencePostalOfficeFK      = physicalEntity.ResidencePostalOfficeFK;
            physicalEntityView.ResidencePlaceFK             = physicalEntity.ResidencePlaceFK;
            physicalEntityView.ResidencePlace               = physicalEntity.ResidencePlace;
            physicalEntityView.ResidenceStreetName          = physicalEntity.ResidenceStreetName;

            physicalEntityView.IdentityCardNumber           = physicalEntity.IdentityCardNumber;
            physicalEntityView.IdentityCardDateOfIssue      = physicalEntity.IdentityCardDateOfIssue;
            physicalEntityView.IdentityCardRegionalOfficeFK = physicalEntity.IdentityCardRegionalOfficeFK;
            physicalEntityView.IdentityCardDateTillValid    = physicalEntity.IdentityCardDateTillValid;

            physicalEntityView.PassportNumber               = physicalEntity.PassportNumber;
            physicalEntityView.PassportDateOfIssue          = physicalEntity.PassportDateOfIssue;
            physicalEntityView.PassportDateTillValid        = physicalEntity.PassportDateTillValid;
            physicalEntityView.PassportCountryOfIssueFK     = physicalEntity.PassportCountryOfIssueFK;
            physicalEntityView.PassportPlaceOfIssue         = physicalEntity.PassportPlaceOfIssue;

            physicalEntityView.ReferentRegionalOfficeFK     = physicalEntity.ReferentRegionalOfficeFK;
            physicalEntityView.ReferentSubstationFK         = physicalEntity.ReferentSubstationFK;

            physicalEntityView.Owner                        = physicalEntity.Owner;
            physicalEntityView.LegalRepresentative          = physicalEntity.LegalRepresentative;
            physicalEntityView.Referent                     = physicalEntity.Referent;

            physicalEntityView.Deleted                      = physicalEntity.Deleted;
        }

        public void ConvertTo(PhysicalEntityView physicalEntityView, PhysicalEntity physicalEntity) 
        {
            physicalEntity.PhysicalEntityPK                 = physicalEntityView.PhysicalEntityPK;

            physicalEntity.Firstname                        = physicalEntityView.Firstname;
            physicalEntity.Lastname                         = physicalEntityView.Lastname;
            physicalEntity.Gender                           = physicalEntityView.Gender;
            physicalEntity.OIB                              = physicalEntityView.OIB;
            physicalEntity.JMBG                             = physicalEntityView.JMBG;
            physicalEntity.DateOfBirth                      = physicalEntityView.DateOfBirth;
            physicalEntity.CitizenshipFK                    = physicalEntityView.CitizenshipFK;

            physicalEntity.BirthCountryFK                   = physicalEntityView.BirthCountryFK;
            physicalEntity.BirthCountyFK                    = physicalEntityView.BirthCountyFK;
            physicalEntity.BirthCityCommunityFK             = physicalEntityView.BirthCityCommunityFK;
            physicalEntity.BirthPlace                       = physicalEntityView.BirthPlace;

            physicalEntity.ResidenceCountryFK               = physicalEntityView.ResidenceCountryFK;
            physicalEntity.ResidenceCountyFK                = physicalEntityView.ResidenceCountyFK;
            physicalEntity.ResidenceCityCommunityFK         = physicalEntityView.ResidenceCityCommunityFK;
            physicalEntity.ResidencePostalOfficeFK          = physicalEntityView.ResidencePostalOfficeFK;
            physicalEntity.ResidencePlaceFK                 = physicalEntityView.ResidencePlaceFK;
            physicalEntity.ResidencePlace                   = physicalEntityView.ResidencePlace;
            physicalEntity.ResidenceStreetName              = physicalEntityView.ResidenceStreetName;

            physicalEntity.IdentityCardNumber               = physicalEntityView.IdentityCardNumber;
            physicalEntity.IdentityCardDateOfIssue          = physicalEntityView.IdentityCardDateOfIssue;
            physicalEntity.IdentityCardRegionalOfficeFK     = physicalEntityView.IdentityCardRegionalOfficeFK;
            physicalEntity.IdentityCardDateTillValid        = physicalEntityView.IdentityCardDateTillValid;

            physicalEntity.PassportNumber                   = physicalEntityView.PassportNumber;
            physicalEntity.PassportDateOfIssue              = physicalEntityView.PassportDateOfIssue;
            physicalEntity.PassportDateTillValid            = physicalEntityView.PassportDateTillValid;
            physicalEntity.PassportCountryOfIssueFK         = physicalEntityView.PassportCountryOfIssueFK;
            physicalEntity.PassportPlaceOfIssue             = physicalEntityView.PassportPlaceOfIssue;

            physicalEntity.ReferentRegionalOfficeFK         = physicalEntityView.ReferentRegionalOfficeFK;
            physicalEntity.ReferentSubstationFK             = physicalEntityView.ReferentSubstationFK;

            physicalEntity.Owner                            = physicalEntityView.Owner;
            physicalEntity.LegalRepresentative              = physicalEntityView.LegalRepresentative;
            physicalEntity.Referent                         = physicalEntityView.Referent;

            physicalEntity.ChangeDate                     = physicalEntityView.ChangeDate;

            physicalEntity.Deleted                          = physicalEntityView.Deleted;
        }

        public void BindDDLs(PhysicalEntityView physicalEntityView, ObjectContext db) 
        {
            //countries ddl
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            physicalEntityView.Citizenships = new SelectList(countriesRepository.GetCitizenships().ToList(), "CountryPK", "Citizenship");
            physicalEntityView.BirthCountries = new SelectList(countriesRepository.GetValid().OrderBy("Name ASC").ToList(), "CountryPK", "Name");
            physicalEntityView.ResidenceCountries = new SelectList(countriesRepository.GetValid().OrderBy("Name ASC").ToList(), "CountryPK", "Name");
            physicalEntityView.PassportCountriesOfIssue = new SelectList(countriesRepository.GetValid().OrderBy("Name ASC").ToList(), "CountryPK", "Name");

            //counties ddl
            if (physicalEntityView.BirthCountryFK != null)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                physicalEntityView.BirthCounties = new SelectList(countiesRepository.GetCountiesByCountry(Convert.ToInt32(physicalEntityView.BirthCountryFK)), "CountyPK", "Name", physicalEntityView.BirthCountyFK);
            }
            else
            {
                physicalEntityView.BirthCounties = new SelectList(new List<County>(), "CountyPK", "Name");
            }

            if (physicalEntityView.ResidenceCountryFK != null)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                physicalEntityView.ResidenceCounties = new SelectList(countiesRepository.GetCountiesByCountry(Convert.ToInt32(physicalEntityView.ResidenceCountryFK)), "CountyPK", "Name", physicalEntityView.ResidenceCountyFK);
            }
            else
            {
                physicalEntityView.ResidenceCounties = new SelectList(new List<County>(), "CountyPK", "Name");
            }

            //citiesCommunities ddl
            if (physicalEntityView.BirthCountyFK != null)
            {
                ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);
                physicalEntityView.BirthCitiesCommunities = new SelectList(citiesCommunitiesRepository.GetCitiesCommunitiesByCounty(Convert.ToInt32(physicalEntityView.BirthCountyFK)), "CityCommunityPK", "Name", physicalEntityView.BirthCityCommunityFK);
            }
            else
            {
                physicalEntityView.BirthCitiesCommunities = new SelectList(new List<CityCommunity>(), "CityCommunityPK", "Name");
            }

            if (physicalEntityView.ResidenceCountyFK != null)
            {
                ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);
                physicalEntityView.ResidenceCitiesCommunities = new SelectList(citiesCommunitiesRepository.GetCitiesCommunitiesByCounty(Convert.ToInt32(physicalEntityView.ResidenceCountyFK)), "CityCommunityPK", "Name", physicalEntityView.ResidenceCityCommunityFK);
            }
            else
            {
                physicalEntityView.ResidenceCitiesCommunities = new SelectList(new List<CityCommunity>(), "CityCommunityPK", "Name");
            }

            //postal offices
            if (physicalEntityView.ResidenceCountyFK != null)
            {
                IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
                var postalOffices = postalOfficesRepository.GetValidByCounty(Convert.ToInt32(physicalEntityView.ResidenceCountyFK)).OrderBy(c => c.Name);

                physicalEntityView.ResidencePostalOffices = new SelectList(postalOffices.Select(c => new { value = c.PostalOfficePK, text = c.Name + " (" + SqlFunctions.StringConvert((double)c.Number).Trim() + ")" }), "value", "text", physicalEntityView.ResidencePostalOfficeFK);
            }
            else
            {
                physicalEntityView.ResidencePostalOffices = new SelectList(new List<PostalOffice>(), "PostalOfficePK", "Name");
            }

            //places
            if (physicalEntityView.ResidencePostalOfficeFK != null)
            {
                IPlacesRepository placesRepository = new PlacesRepository(db);
                physicalEntityView.ResidencePlaces = new SelectList(placesRepository.GetPlacesByPostalOffice(Convert.ToInt32(physicalEntityView.ResidencePostalOfficeFK)), "PlacePK", "Name", physicalEntityView.ResidencePlaceFK);
            }
            else
            {
                physicalEntityView.ResidencePlaces = new SelectList(new List<Place>(), "PlacePK", "Name");
            }

            //regional offices ddl
            IRegionalOfficesRepository regionalOfficesView = new RegionalOfficesRepository(db);
            physicalEntityView.IdentityCardRegionalOffices = new SelectList(regionalOfficesView.GetValid().ToList(), "RegionalOfficePK", "Name", physicalEntityView.IdentityCardRegionalOfficeFK);
            physicalEntityView.ReferentRegionalOffices = new SelectList(regionalOfficesView.GetValid().ToList(), "RegionalOfficePK", "Name", physicalEntityView.ReferentRegionalOfficeFK);

            //substations
            if (physicalEntityView.ReferentRegionalOfficeFK != null)
            {
                ISubstationsRepository substationsRepository = new SubstationsRepository(db);
                physicalEntityView.ReferentSubstations = new SelectList(substationsRepository.GetValidByRegionalOffice(Convert.ToInt32(physicalEntityView.ReferentRegionalOfficeFK)), "SubstationPK", "Name", physicalEntityView.ReferentSubstationFK);
            }
            else
            {
                physicalEntityView.ReferentSubstations = new SelectList(new List<Place>(), "SubstationPK", "Name");
            }

            // genders dll
            physicalEntityView.Genders = new SelectList(GeneratorView.GenerateGenders(), "Value", "Text");
        }


        public static IQueryable<PhysicalEntityView> GetPhysicalEntityView(IQueryable<PhysicalEntity> physicalEntityTable) 
        {
            IQueryable<PhysicalEntityView> physicalEntityViewList = (from t1 in physicalEntityTable

                                       select new PhysicalEntityView
                                       {
                                            PhysicalEntityPK        = t1.PhysicalEntityPK,
                                            Firstname               = t1.Firstname,
                                            Lastname                = t1.Lastname,
                                            DateOfBirth             = t1.DateOfBirth,
                                            LegalRepresentative     = t1.LegalRepresentative,
                                            Owner                   = t1.Owner,
                                            Referent                = t1.Referent,
                                            OIB                     = t1.OIB,
                                       }).AsQueryable<PhysicalEntityView>();

            return physicalEntityViewList;
        }


        public static IQueryable<PhysicalEntityView> GetPhysicalEntitySelect(IQueryable<PhysicalEntity> physicalEntityTable) 
        {
            IQueryable<PhysicalEntityView> physicalEntityViewList = (from t1 in physicalEntityTable

                                       select new PhysicalEntityView
                                       {
                                            PhysicalEntityPK        = t1.PhysicalEntityPK,
                                            Name                    = t1.Lastname + " " + t1.Firstname,
                                       }).AsQueryable<PhysicalEntityView>();

            return physicalEntityViewList.OrderBy(o => o.Name);
        }
    }
}
