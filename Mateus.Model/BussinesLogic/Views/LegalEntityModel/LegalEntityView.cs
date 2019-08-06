using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.BussinesLogic.Support.Validation;
using System.Data.Objects;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository;
using System.Data.Objects.SqlClient;
using Mateus.Model.BussinesLogic.Views.PhysicalEntityModel;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityModel
{
    public class LegalEntityView
    {
        public int LegalEntityPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Kratki naziv je obavezan."), StringLength(3, ErrorMessage = "Kratki naziv ne smije biti duži od 3 znaka.")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "OIB je obavezan."), OIB(ErrorMessage="OIB nije ispravnan.")]
        public string OIB { get; set; }

        [StringLength(8, ErrorMessage = "MB ne smije biti duži od 8 znakova.")]
        public string MB { get; set; }

        [Required(ErrorMessage = "MBS je obavezan."), StringLength(9, ErrorMessage = "MBS ne smije biti duži od 8 znakova.")]
        public string MBS { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Djelatnost je obavezna.")]
        public int? ActivityFK { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Opis stvarne djelatnosti je obavezan."), StringLength(256, ErrorMessage = "Opis stvarne djelatnosti ne smije biti duži od 256 znakova.")]
        public string ActivityDescription { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Oblik je obavezan.")]
        public int? FormFK { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Temeljni kapital je obavezan.")]
        public decimal? FundamentalCapital { get; set; }

        [Required(ErrorMessage = "Država je obavezna.")]
        public int? CountryFK { get; set; }

        [RequiredIf("CountryFK == 81", ErrorMessage = "Županija je obavezna.")]
        public int? CountyFK { get; set; }

        [RequiredIf("CountryFK == 81", ErrorMessage = "Grad/općina je obavezan.")]
        public int? CityCommunityFK { get; set; }

        [RequiredIf("CountryFK == 81", ErrorMessage = "Poštanski broj je obavezan.")]
        public int? PostalOfficeFK { get; set; }

        [RequiredIf("CountryFK == 81", ErrorMessage = "Naselje je obavezno.")]
        public int? PlaceFK { get; set; }

        [RequiredIf("CountryFK != 81", ErrorMessage = "Mjesto je obavezno.")]
        public string Place { get; set; }

        [Required(ErrorMessage = "Ulica i broj su obavezni.")]
        public string StreetName { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Područni ured je obavezan.")]
        public int? RegionalOfficeFK { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Ispostava je obavezna.")]
        public int? SubstationFK { get; set; }

        public int? ReferentFK { get; set; }

        public DateTime? DateOfRegistration { get; set; }

        [Required(ErrorMessage = "Trgovački sud je obavezan.")]
        public int? CommercialCourtFK { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Porez je obavezan.")]
        public int? TaxFK { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }

        [Email(ErrorMessage = "E-mail adresa nije valjana.")]
        public string EMail { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Datum prvog kontakta je obavezan.")]
        public DateTime? FirstContactDate { get; set; }

        public bool? TouristOffice { get; set; }
        public string TouristOfficeDescription { get; set; }
        public bool? MonumentAnnuity { get; set; }
        public string MonumentAnnuityDescription { get; set; }
        public int? NumberOfEmployees { get; set; }

        [RequiredIf("Company == true && NumberOfEmployees > 0", ErrorMessage="MIRO - registracijski broj je obavezan."), StringLength(10, ErrorMessage="MIRO - registracijski broj ne smije biti duži od 10 znakova.")]
        public string MIORRegistrationNumber { get; set; }

        [RequiredIf("Company == true && NumberOfEmployees > 0", ErrorMessage="HZZO - broj obveze je obavezan."), StringLength(11, ErrorMessage="HZZO - broj obveze ne smije biti duži od 11 znakova.")]
        public string HZZOObligationNumber { get; set; }

        [RequiredIf("Company == true && NumberOfEmployees > 0", ErrorMessage="HZZO - šifra poslovnog subjekta je obavezna."), StringLength(10, ErrorMessage="HZZO - šifra poslovnog subjekta ne smije biti duža od 10 znakova.")]
        public string HZZOBussinesEntityCode { get; set; }

        [Required(ErrorMessage = "Datum rješenja je obavezan.")]
        public DateTime? ChangeDate { get; set; }

        public bool? Owner { get; set; }
        public bool? Company { get; set; }
        public bool? Active { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Forms { get; set; }
        public IEnumerable<SelectListItem> Activities { get; set; }
        public IEnumerable<SelectListItem> Countries { get; set; }
        public IEnumerable<SelectListItem> Counties { get; set; }
        public IEnumerable<SelectListItem> CitiesCommunities { get; set; }
        public IEnumerable<SelectListItem> PostalOffices { get; set; }
        public IEnumerable<SelectListItem> Places { get; set; }
        public IEnumerable<SelectListItem> RegionalOffices { get; set; }
        public IEnumerable<SelectListItem> Substations { get; set; }
        public IEnumerable<SelectListItem> Referents { get; set; }
        public IEnumerable<SelectListItem> CommercialCourts { get; set; }
        public IEnumerable<SelectListItem> Taxes { get; set; }
        public IEnumerable<SelectListItem> Risks { get; set; }

        public int? BranchesCount { get; set; }
        public int? ContractsCount { get; set; }
        public int? BanksCount { get; set; }
        public int? LegalRepresentativesCount { get; set; }
        public int? LegalEntityOwnersCount { get; set; }
        public int? AssessmentsCount { get; set; }

        public string FormName { get; set; }
        public string ActivityName { get; set; }

        public string RegionalOfficeName { get; set; }
        public string SubstationName { get; set; }
        public string ReferentName { get; set; }
        public string CommercialCourtName { get; set; }
        public string TaxName { get; set; }

        public string CountryName { get; set; }
        public string CountyName { get; set; }
        public string CityCommunityName { get; set; }
        public string PostalOfficeName { get; set; }
        public string PlaceName { get; set; }

        public string BankName { get; set; }
        public int? BankFK { get; set; }

        public void ConvertFrom(LegalEntity legalEntity, LegalEntityView legalEntityView) 
        {
            legalEntityView.LegalEntityPK               = legalEntity.LegalEntityPK;
            legalEntityView.Name                        = legalEntity.Name;
            legalEntityView.ShortName                   = legalEntity.ShortName;
            legalEntityView.OIB                         = legalEntity.OIB;
            legalEntityView.MB                          = legalEntity.MB;
            legalEntityView.MBS                         = legalEntity.MBS;

            legalEntityView.ActivityFK                  = legalEntity.ActivityFK;
            legalEntityView.ActivityName                = legalEntity.Activity != null ? legalEntity.Activity.Name : null;
            legalEntityView.ActivityDescription         = legalEntity.ActivityDescription;
            legalEntityView.FormFK                      = legalEntity.FormFK;
            legalEntityView.FundamentalCapital          = legalEntity.FundamentalCapital;
            legalEntityView.CountryFK                   = legalEntity.CountryFK;
            legalEntityView.CountyFK                    = legalEntity.CountyFK;
            legalEntityView.CityCommunityFK             = legalEntity.CityCommunityFK;
            legalEntityView.PlaceFK                     = legalEntity.PlaceFK;
            legalEntityView.Place                       = legalEntity.Place;
            legalEntityView.StreetName                  = legalEntity.StreetName;

            legalEntityView.PostalOfficeFK              = legalEntity.PostalOfficeFK;
            legalEntityView.RegionalOfficeFK            = legalEntity.RegionalOfficeFK;
            legalEntityView.SubstationFK                = legalEntity.SubstationFK;
            legalEntityView.ReferentFK                  = legalEntity.ReferentFK;
            legalEntityView.DateOfRegistration          = legalEntity.DateOfRegistration;
            legalEntityView.CommercialCourtFK           = legalEntity.CommercialCourtFK;
            legalEntityView.TaxFK                       = legalEntity.TaxFK;

            legalEntityView.Phone                       = legalEntity.Phone;
            legalEntityView.Fax                         = legalEntity.Fax;
            legalEntityView.Mobile                      = legalEntity.Mobile;
            legalEntityView.EMail                       = legalEntity.EMail;

            legalEntityView.TouristOffice               = legalEntity.TouristOffice;
            legalEntityView.TouristOfficeDescription    = legalEntity.TouristOfficeDescription;
            legalEntityView.FirstContactDate            = legalEntity.FirstContactDate;
            legalEntityView.MonumentAnnuity             = legalEntity.MonumentAnnuity;
            legalEntityView.MonumentAnnuityDescription  = legalEntity.MonumentAnnuityDescription;
            legalEntityView.NumberOfEmployees           = legalEntity.NumberOfEmployees;
            legalEntityView.MIORRegistrationNumber      = legalEntity.MIORRegistrationNumber;
            legalEntityView.HZZOObligationNumber        = legalEntity.HZZOObligationNumber;
            legalEntityView.HZZOBussinesEntityCode      = legalEntity.HZZOBussinesEntityCode;

            legalEntityView.Owner                       = legalEntity.Owner;
            legalEntityView.Company                     = legalEntity.Company;
            legalEntityView.Active                      = legalEntity.Active;

            legalEntityView.Deleted                     = legalEntity.Deleted;
        }

        public void ConvertTo(LegalEntityView legalEntityView, LegalEntity legalEntity) 
        {
            legalEntity.LegalEntityPK               = legalEntityView.LegalEntityPK;
            legalEntity.Name                        = legalEntityView.Name;
            legalEntity.ShortName                   = legalEntityView.ShortName;
            legalEntity.OIB                         = legalEntityView.OIB;
            legalEntity.MB                          = legalEntityView.MB;
            legalEntity.MBS                         = legalEntityView.MBS;

            legalEntity.ActivityFK                  = legalEntityView.ActivityFK;
            legalEntity.ActivityDescription         = legalEntityView.ActivityDescription;
            legalEntity.FormFK                      = legalEntityView.FormFK;
            legalEntity.FundamentalCapital          = legalEntityView.FundamentalCapital;
            legalEntity.CountryFK                   = legalEntityView.CountryFK;
            legalEntity.CountyFK                    = legalEntityView.CountyFK;
            legalEntity.CityCommunityFK             = legalEntityView.CityCommunityFK;
            legalEntity.PlaceFK                     = legalEntityView.PlaceFK;
            legalEntity.Place                       = legalEntityView.Place;
            legalEntity.StreetName                  = legalEntityView.StreetName;

            legalEntity.PostalOfficeFK              = legalEntityView.PostalOfficeFK;
            legalEntity.RegionalOfficeFK            = legalEntityView.RegionalOfficeFK;
            legalEntity.SubstationFK                = legalEntityView.SubstationFK;
            legalEntity.ReferentFK                  = legalEntityView.ReferentFK;
            legalEntity.DateOfRegistration          = legalEntityView.DateOfRegistration;
            legalEntity.CommercialCourtFK           = legalEntityView.CommercialCourtFK;
            legalEntity.TaxFK                       = legalEntityView.TaxFK;

            legalEntity.Phone                       = legalEntityView.Phone;
            legalEntity.Fax                         = legalEntityView.Fax;
            legalEntity.Mobile                      = legalEntityView.Mobile;
            legalEntity.EMail                       = legalEntityView.EMail;

            legalEntity.TouristOffice               = legalEntityView.TouristOffice;
            legalEntity.TouristOfficeDescription    = legalEntityView.TouristOfficeDescription;
            legalEntity.FirstContactDate            = legalEntityView.FirstContactDate;
            legalEntity.MonumentAnnuity             = legalEntityView.MonumentAnnuity;
            legalEntity.MonumentAnnuityDescription  = legalEntityView.MonumentAnnuityDescription;
            legalEntity.NumberOfEmployees           = legalEntityView.NumberOfEmployees;
            legalEntity.MIORRegistrationNumber      = legalEntityView.MIORRegistrationNumber;
            legalEntity.HZZOObligationNumber        = legalEntityView.HZZOObligationNumber;
            legalEntity.HZZOBussinesEntityCode      = legalEntityView.HZZOBussinesEntityCode;

            legalEntity.ChangeDate                  = legalEntityView.ChangeDate;

            legalEntity.Owner                       = legalEntityView.Owner;
            legalEntity.Company                     = legalEntityView.Company;
            legalEntity.Active                      = legalEntityView.Active;

            legalEntity.Deleted                     = legalEntityView.Deleted;
        }

        public void BindDDLs(LegalEntityView legalEntityView, ObjectContext db) 
        {
            //forms ddl
            IFormsRepository formsRepository = new FormsRepository(db);
            legalEntityView.Forms = new SelectList(formsRepository.GetValid().ToList(), "FormPK", "Name");

            //activities ddl
            IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);
            var activities = activitiesRepository.GetValid().ToList();
            legalEntityView.Activities = new SelectList(activities.Select(c => new { value = c.ActivityPK, text = c.Name + " (" + c.Code + ")" }), "value", "text");

            //countries ddl
            ICountriesRepository countriesRepository = new CountriesRepository(db);
            legalEntityView.Countries = new SelectList(countriesRepository.GetValid().ToList(), "CountryPK", "Name");

            //counties ddl
            if (legalEntityView.CountyFK != null)
            {
                ICountiesRepository countiesRepository = new CountiesRepository(db);
                legalEntityView.Counties = new SelectList(countiesRepository.GetCountiesByCountry((int)legalEntityView.CountryFK).ToList(), "CountyPK", "Name");
            }
            else
            {
                legalEntityView.Counties = new SelectList(new List<CityCommunity>(), "CountyPK", "Name");
            }

            //regional offices ddl
            IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);
            legalEntityView.RegionalOffices = new SelectList(regionalOfficesRepository.GetValid().ToList(), "RegionalOfficePK", "Name");

            //commercial courts ddl
            ICommercialCourtsRepository commercialCourtsRepository = new CommercialCourtsRepository(db);
            legalEntityView.CommercialCourts = new SelectList(commercialCourtsRepository.GetValid().ToList(), "CommercialCourtPK", "Name");

            //taxes ddl
            ITaxesRepository taxesRepository = new TaxesRepository(db);
            legalEntityView.Taxes = new SelectList(taxesRepository.GetValid().ToList(), "TaxPK", "Name");

            //citiesCommunities dll
            if (legalEntityView.CountyFK != null)
            {
                ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(db);
                legalEntityView.CitiesCommunities = new SelectList(citiesCommunitiesRepository.GetCitiesCommunitiesByCounty(Convert.ToInt32(legalEntityView.CountyFK)), "CityCommunityPK", "Name", legalEntityView.CityCommunityFK);
            }
            else
            {
                legalEntityView.CitiesCommunities = new SelectList(new List<CityCommunity>(), "CityCommunityPK", "Name");
            }

            //postal offices dll
            if (legalEntityView.CountyFK != null)
            {
                IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
                var postalOffices = postalOfficesRepository.GetValidByCounty(Convert.ToInt32(legalEntityView.CountyFK)).OrderBy(c => c.Name);

                legalEntityView.PostalOffices = new SelectList(postalOffices.Select(c => new { value = c.PostalOfficePK, text = c.Name + " (" + SqlFunctions.StringConvert((double)c.Number).Trim() + ")" }), "value", "text", legalEntityView.PostalOfficeFK);
            }
            else
            {
                legalEntityView.PostalOffices = new SelectList(new List<PostalOffice>(), "PostalOfficePK", "Name");
            }

            //places dll
            if (legalEntityView.PostalOfficeFK != null)
            {
                IPlacesRepository placesRepository = new PlacesRepository(db);
                legalEntityView.Places = new SelectList(placesRepository.GetPlacesByPostalOffice(Convert.ToInt32(legalEntityView.PostalOfficeFK)), "PlacePK", "Name", legalEntityView.PlaceFK);
            }
            else
            {
                legalEntityView.Places = new SelectList(new List<Place>(), "PlacePK", "Name");
            }

            //substations dll
            if (legalEntityView.RegionalOfficeFK != null)
            {
                ISubstationsRepository substationsRepository = new SubstationsRepository(db);
                legalEntityView.Substations = new SelectList(substationsRepository.GetValidByRegionalOffice(Convert.ToInt32(legalEntityView.RegionalOfficeFK)), "SubstationPK", "Name", legalEntityView.SubstationFK);
            }
            else
            {
                legalEntityView.Substations = new SelectList(new List<Substation>(), "SubstationPK", "Name");
            }

            //referents dll
            if (legalEntityView.SubstationFK != null)
            {
                //physicalEntity ddl
                PhysicalEntityView PhysicalEntityView = new PhysicalEntityView();
                IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
                legalEntityView.Referents = new SelectList(PhysicalEntityView.GetPhysicalEntitySelect(physicalEntitiesRepository.GetValidReferentsBySubstation((int)legalEntityView.SubstationFK)).ToList(), "PhysicalEntityPK", "Name");
            }
            else
            {
                legalEntityView.Referents = new SelectList(new List<PhysicalEntity>(), "PhysicalEntityPK", "Name");
            }
        }

        public static IQueryable<LegalEntityView> GetLegalEntityView(
                                    IQueryable<LegalEntity> legalEntityTable, 
                                    IQueryable<LegalEntityBranch> branchTable, 
                                    IQueryable<Contract> contractTable, 
                                    IQueryable<Bank> bankTable, 
                                    IQueryable<LegalEntityBank> legalEntityBankTable, 
                                    IQueryable<LegalEntityLegalRepresentative> legalEntityLegalRepresentativeTable, 
                                    IQueryable<LegalEntityOwner> legalEntityOwnerTable,
                                    IQueryable<Assessment> assessmentTable) 
        {
            IQueryable<LegalEntityView> legalEntityViewList = (from t1 in legalEntityTable
                                                       select new LegalEntityView
                                                       {
                                                            LegalEntityPK = t1.LegalEntityPK,
                                                            Name = t1.Name + " (" + t1.OIB + ")",
                                                            Owner = t1.Owner,
                                                            Company = t1.Company,
                                                            BranchesCount = branchTable.Where(b => b.LegalEntityFK == t1.LegalEntityPK).Count(),
                                                            ContractsCount = contractTable.Where(c => c.LegalEntityFK == t1.LegalEntityPK).Count(),
                                                            BanksCount = (from mn in legalEntityBankTable 
                                                                          from c1 in legalEntityTable.Where(c2 => c2.LegalEntityPK == mn.LegalEntityFK).DefaultIfEmpty()
                                                                          from b1 in bankTable.Where( b2 => b2.BankPK == mn.BankFK).DefaultIfEmpty()
                                                                          where c1.LegalEntityPK == t1.LegalEntityPK
                                                                          select mn).Count(),
                                                            LegalRepresentativesCount = legalEntityLegalRepresentativeTable.Where(l => l.LegalEntityFK == t1.LegalEntityPK).Count(),
                                                            LegalEntityOwnersCount = legalEntityOwnerTable.Where(o => o.LegalEntityFK == t1.LegalEntityPK).Count(),
                                                            AssessmentsCount = assessmentTable.Where(a => a.LegalEntityFK == t1.LegalEntityPK).Count()                                                   
                                                       }).AsQueryable<LegalEntityView>();

            return legalEntityViewList;
        }

        public static IQueryable<LegalEntityView> GetLegalEntityReportView(IQueryable<LegalEntity> legalEntityTable)
        {
            IQueryable<LegalEntityView> legalEntityViewList = (from t1 in legalEntityTable
                                                       select new LegalEntityView
                                                       {
                                                           LegalEntityPK = t1.LegalEntityPK,
                                                           Name = t1.Name,
                                                           OIB = t1.OIB
                                                       }).AsQueryable<LegalEntityView>();

            return legalEntityViewList;
        }

        public static IQueryable<LegalEntityView> GetLegalEntitiesReport(ObjectContext db, string Name, string OIB, string MB, string MBS, int? BankPK, int? TaxPK, int? FormPK, int? ActivityPK, int? SubstationPK, int? CommercialCourtPK, int? numberOfEmployeesFrom, int? numberOfEmployeesTo, int? fundamentalCapitalFrom, int? fundamentalCapitalTo, bool? TouristOffice, bool? MonumentAnnuity)
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IQueryable<LegalEntity> legalEntityTable = legalEntitiesRepository.GetValidLegalEntities();

            IBanksRepository banksRepository = new BanksRepository(db);
            ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);

            var banksTable = banksRepository.GetValid();
            var legalEntitiesBanksTable = legalEntitiesBanksRepository.GetValid();

            IQueryable<LegalEntity> legalEntitiesFiltered = legalEntitiesRepository.GetValid();

            if(legalEntitiesFiltered.Count() > 0) 
            {
                if (!String.IsNullOrWhiteSpace(Name)) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByName(Name); }
                if (!String.IsNullOrWhiteSpace(OIB)) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByOIB(OIB); }
                if (!String.IsNullOrWhiteSpace(MB)) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByMB(MB); }
                if (!String.IsNullOrWhiteSpace(MBS)) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByMBS(MBS); }

                if (numberOfEmployeesFrom != null || numberOfEmployeesTo != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByEmployeesRange(numberOfEmployeesFrom, numberOfEmployeesTo); }
                if (fundamentalCapitalFrom != null || fundamentalCapitalTo != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByFundamentalCapitalRange(fundamentalCapitalFrom, fundamentalCapitalTo); }

                if (BankPK != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByBank(db, BankPK); }
                if (TaxPK != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByTax(TaxPK); }

                if (SubstationPK != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesBySubstation(SubstationPK); }
                if (CommercialCourtPK != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByCommercialCourt(CommercialCourtPK); }

                if (FormPK != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByForm(FormPK); }
                if (ActivityPK != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByActivity(ActivityPK); }

                if (TouristOffice != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByTouristOffice(TouristOffice); }
                if (MonumentAnnuity != null) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByMonumentAnnuity(MonumentAnnuity); }
            }    

            IQueryable<LegalEntityView> legalEntityViewList = (from t1 in legalEntitiesFiltered
                                                            from t2 in legalEntitiesBanksTable.Where(b => b.LegalEntityFK == t1.LegalEntityPK).DefaultIfEmpty()
                                                            from t3 in banksTable.Where(b => b.BankPK == t2.BankFK).DefaultIfEmpty()
                                                        select new LegalEntityView
                                                        {
                                                            LegalEntityPK = t1.LegalEntityPK,

                                                            Name = t1.Name,
                                                            ShortName = t1.ShortName != null ? t1.ShortName : "",
                                                            OIB = t1.OIB != null ? t1.OIB : "",
                                                            MB = t1.MB != null ? t1.MB : "",
                                                            MBS = t1.MBS != null ? t1.MBS : "",
                                                            FormName = t1.Form.Name != null ? t1.Form.Name : "",
                                                            ActivityName = t1.Activity.Name != null ? t1.Activity.Name : "",
                                                            ActivityDescription = t1.ActivityDescription != null ? t1.ActivityDescription : "",
                                                            FundamentalCapital = t1.FundamentalCapital,

                                                            RegionalOfficeName = t1.RegionalOffice.Name != null ? t1.RegionalOffice.Name : "",
                                                            SubstationName = t1.Substation.Name != null ? t1.Substation.Name : "",
                                                            ReferentName = t1.PhysicalEntity.Firstname != null ? t1.PhysicalEntity.Firstname + " " + t1.PhysicalEntity.Lastname : "",
                                                            DateOfRegistration = t1.DateOfRegistration,
                                                            CommercialCourtName = t1.CommercialCourt.Name != null ? t1.CommercialCourt.Name : "",
                                                            TaxName = t1.Tax.Name != null ? t1.Tax.Name : "",

                                                            ChangeDate = t1.ChangeDate,

                                                            CountryName = t1.Country.Name != null ? t1.Country.Name : "",
                                                            CountyName = t1.County.Name != null ? t1.County.Name : "",
                                                            CityCommunityName = t1.CitiesCommunity.Name != null ? t1.CitiesCommunity.Name : "",
                                                            PostalOfficeName = t1.PostalOffice.Name != null ? t1.Phone : "",
                                                            PlaceName = t1.Place1.Name != null ? t1.Place1.Name : t1.Place,
                                                            StreetName = t1.StreetName,

                                                            Phone = t1.Phone != null ? t1.Phone : "",
                                                            Fax = t1.Fax != null ? t1.Fax : "",
                                                            Mobile = t1.Mobile != null ? t1.Mobile : "",
                                                            EMail = t1.EMail != null ? t1.EMail : ""

                                                        }).Distinct().AsQueryable<LegalEntityView>();

            return legalEntityViewList;
        }

        public static LegalEntityView GetLegalEntityReport(ObjectContext db, int legalEntityPK)
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK(legalEntityPK);

            LegalEntityView legalEntityView = new LegalEntityView(); 

            legalEntityView.LegalEntityPK = legalEntity.LegalEntityPK;
            legalEntityView.Name = legalEntity.Name;
            legalEntityView.ShortName = legalEntity.ShortName;
            legalEntityView.OIB = legalEntity.OIB;
            legalEntityView.MB = legalEntity.MB;
            legalEntityView.MBS = legalEntity.MBS;
            legalEntityView.ActivityName = legalEntity.ActivityFK != null ? legalEntity.Activity.Name : null;
            legalEntityView.ActivityDescription = legalEntity.ActivityDescription;
            legalEntityView.FormName = legalEntity.FormFK != null ? legalEntity.Form.Name : null;
            legalEntityView.FundamentalCapital = legalEntity.FundamentalCapital;

            legalEntityView.CountryName = legalEntity.Country.Name;
            legalEntityView.CountyName = legalEntity.CountyFK != null ? legalEntity.County.Name : null;
            legalEntityView.PlaceName = legalEntity.PlaceFK != null ? legalEntity.Place1.Name : null;
            legalEntityView.CityCommunityName = legalEntity.CityCommunityFK != null ? legalEntity.CitiesCommunity.Name : null;
            legalEntityView.StreetName = legalEntity.StreetName;

            legalEntityView.PostalOfficeName = legalEntity.PostalOfficeFK != null ? legalEntity.PostalOffice.Name : null;
            legalEntityView.RegionalOfficeName = legalEntity.RegionalOfficeFK != null ? legalEntity.RegionalOffice.Name : null;
            legalEntityView.SubstationName = legalEntity.SubstationFK != null ? legalEntity.Substation.Name : null;
            legalEntityView.ReferentName = legalEntity.ReferentFK != null ? legalEntity.PhysicalEntity.Firstname + " " + legalEntity.PhysicalEntity.Lastname : null;
            legalEntityView.DateOfRegistration = legalEntity.DateOfRegistration;
            legalEntityView.CommercialCourtName = legalEntity.CommercialCourtFK != null ? legalEntity.CommercialCourt.Name : null;
            legalEntityView.TaxName = legalEntity.TaxFK != null ? legalEntity.Tax.Name : null;

            legalEntityView.Phone = legalEntity.Phone;
            legalEntityView.Fax = legalEntity.Fax;
            legalEntityView.Mobile = legalEntity.Mobile;
            legalEntityView.EMail = legalEntity.EMail;

            legalEntityView.TouristOffice = legalEntity.TouristOffice;
            legalEntityView.TouristOfficeDescription = legalEntity.TouristOfficeDescription;
            legalEntityView.FirstContactDate = legalEntity.FirstContactDate;
            legalEntityView.MonumentAnnuity = legalEntity.MonumentAnnuity;
            legalEntityView.MonumentAnnuityDescription = legalEntity.MonumentAnnuityDescription;
            legalEntityView.NumberOfEmployees = legalEntity.NumberOfEmployees;
            legalEntityView.MIORRegistrationNumber = legalEntity.MIORRegistrationNumber;
            legalEntityView.HZZOObligationNumber = legalEntity.HZZOObligationNumber;
            legalEntityView.HZZOBussinesEntityCode = legalEntity.HZZOBussinesEntityCode;
            legalEntityView.Active = legalEntity.Active;
            legalEntityView.Deleted = legalEntity.Deleted;

            return legalEntityView;
        }
    }
}
