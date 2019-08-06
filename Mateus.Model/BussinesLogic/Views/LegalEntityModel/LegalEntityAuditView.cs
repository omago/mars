using System;
using System.Collections.Generic;
using System.Linq;
using Mateus.Model.EFModel;
using System.Data.Objects;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.BussinesLogic.Support.ExtensionMethods;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityAuditModel
{
    public class LegalEntityAuditView
    {
        public int LegalEntityPK { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string OIB { get; set; }
        public string MB { get; set; }
        public string MBS { get; set; }

        public int? ActivityFK { get; set; }
        public string ActivityDescription { get; set; }

        public int? FormFK { get; set; }

        public decimal? FundamentalCapital { get; set; }

        public int? CountryFK { get; set; }
        public int? CountyFK { get; set; }
        public int? CityCommunityFK { get; set; }
        public int? PostalOfficeFK { get; set; }
        public int? PlaceFK { get; set; }
        public string Place { get; set; }
        public string StreetName { get; set; }

        public int? RegionalOfficeFK { get; set; }
        public int? SubstationFK { get; set; }
        public int? ReferentFK { get; set; }

        public DateTime? DateOfRegistration { get; set; }

        public int? CommercialCourtFK { get; set; }
        public int? TaxFK { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string EMail { get; set; }

        public DateTime? FirstContactDate { get; set; }

        public bool? TouristOffice { get; set; }
        public string TouristOfficeDescription { get; set; }
        public bool? MonumentAnnuity { get; set; }
        public string MonumentAnnuityDescription { get; set; }
        public int? NumberOfEmployees { get; set; }

        public string MIORRegistrationNumber { get; set; }
        public string HZZOObligationNumber { get; set; }
        public string HZZOBussinesEntityCode { get; set; }

        public DateTime? ChangeDate { get; set; }

        public bool? Owner { get; set; }
        public bool? LegalEntity { get; set; }
        public bool? Active { get; set; }

        public bool? Deleted { get; set; }

        // names
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

        public static List<LegalEntityAuditView> GetLegalEntityAuditView(ObjectContext context, int relatedEntityPK) 
        {
            IAuditingDetailsRepository auditingDetailsRepository = new AuditingDetailsRepository(context); 
            IAuditingMasterRepository auditingMasterRepository = new AuditingMasterRepository(context);

            List<LegalEntityAuditView> legalEntityAuditViewList = new List<LegalEntityAuditView>();

            var sessionTokens = (from am in auditingMasterRepository.GetAll().Where(c => c.TableName == "LegalEntities")
                                where am.RelatedEntityPK == relatedEntityPK
                                select new { 
                                    auditingMasterPK = am.AuditingMasterPK, 
                                    sessionToken = am.SessionToken 
                                }).ToList();

            foreach (var item in sessionTokens) 
            {
                var record = auditingDetailsRepository.GetAuditingDetailByAuditingMasterPK(item.auditingMasterPK).ToList();

                LegalEntityAuditView leav = new LegalEntityAuditView();

                leav.Name = record.checkString("Name");
                leav.ShortName = record.checkString("ShortName");
                leav.OIB = record.checkString("OIB");
                leav.MB = record.checkString("MB");
                leav.MBS = record.checkString("MBS");
                leav.ActivityFK = record.checkInteger("ActivityFK");
                leav.ActivityDescription = record.checkString("ActivityDescription");
                leav.FormFK = record.checkInteger("FormFK");
                leav.FundamentalCapital = record.checkDecimal("FundamentalCapital");

                leav.CountryFK = record.checkInteger("CountryFK");
                leav.CountyFK = record.checkInteger("CountyFK");
                leav.PlaceFK = record.checkInteger("PlaceFK");
                leav.PostalOfficeFK = record.checkInteger("PostalOfficeFK");
                leav.Place = record.checkString("Place");
                leav.CityCommunityFK = record.checkInteger("CityCommunityFK");
                leav.StreetName = record.checkString("StreetName");

                leav.RegionalOfficeFK = record.checkInteger("RegionalOfficeFK");
                leav.SubstationFK = record.checkInteger("SubstationFK");
                leav.ReferentFK = record.checkInteger("ReferentFK");
                leav.DateOfRegistration = record.checkDate("DateOfRegistration");
                leav.CommercialCourtFK = record.checkInteger("CommercialCourtFK");
                leav.TaxFK = record.checkInteger("TaxFK");

                leav.Phone = record.checkString("Phone");
                leav.Fax = record.checkString("Fax");
                leav.Mobile = record.checkString("Mobile");
                leav.EMail = record.checkString("EMail");

                leav.FirstContactDate = record.checkDate("FirstContactDate");
                leav.TouristOffice = record.checkBoolean("TouristOffice");
                leav.TouristOfficeDescription = record.checkString("TouristOfficeDescription");
                leav.MonumentAnnuity = record.checkBoolean("MonumentAnnuity");
                leav.MonumentAnnuityDescription = record.checkString("MonumentAnnuityDescription");
                leav.NumberOfEmployees = record.checkInteger("NumberOfEmployees");
                leav.MIORRegistrationNumber = record.checkString("MIORRegistrationNumber");
                leav.HZZOObligationNumber = record.checkString("HZZOObligationNumber");
                leav.HZZOBussinesEntityCode = record.checkString("HZZOBussinesEntityCode");

                leav.ChangeDate = record.checkDate("ChangeDate");

                leav.Owner = record.checkBoolean("Owner");
                leav.LegalEntity = record.checkBoolean("LegalEntity");
                leav.Active = record.checkBoolean("Active");

                legalEntityAuditViewList.Add(leav);
            }

            // Connect all foreign keys and return data collection as List<LegalEntityAuditView>
            IFormsRepository formsRepository = new FormsRepository(context);
            IQueryable<Form> formsTable = formsRepository.GetValid();

            IActivitiesRepository activitiesRepository = new ActivitiesRepository(context);
            IQueryable<Activity> activitiesTable = activitiesRepository.GetValid();

            IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(context);
            IQueryable<RegionalOffice> regionalOfficesTable = regionalOfficesRepository.GetValid();

            ISubstationsRepository substationsRepository = new SubstationsRepository(context);
            IQueryable<Substation> substationsTable = substationsRepository.GetValid();

            ICommercialCourtsRepository commercialCourtsRepository = new CommercialCourtsRepository(context);
            IQueryable<CommercialCourt> commercialCourtsTable = commercialCourtsRepository.GetValid();

            ITaxesRepository taxesRepository = new TaxesRepository(context);
            IQueryable<Tax> taxesTable = taxesRepository.GetValid();

            ICountriesRepository countriesRepository = new CountriesRepository(context);
            IQueryable<Country> countriesTable = countriesRepository.GetValid();

            ICountiesRepository countiesRepository = new CountiesRepository(context);
            IQueryable<County> countiesTable = countiesRepository.GetValid();

            ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(context);
            IQueryable<CityCommunity> cityCommunityTable = citiesCommunitiesRepository.GetValid();

            IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(context);
            IQueryable<PostalOffice> postalOfficeTable = postalOfficesRepository.GetValid();

            IPlacesRepository placesRepository = new PlacesRepository(context);
            IQueryable<Place> placesTable = placesRepository.GetValid();

            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(context);
            IQueryable<PhysicalEntity> physicalEntitiesTable = physicalEntitiesRepository.GetValid();

            List<LegalEntityAuditView> legalEntityAuditView =  
                                              (from t in legalEntityAuditViewList
                                               from t1 in formsTable.Where(tbl => tbl.FormPK == t.FormFK).DefaultIfEmpty()
                                               from t2 in activitiesTable.Where(tbl => tbl.ActivityPK == t.ActivityFK).DefaultIfEmpty()
                                               from t3 in regionalOfficesTable.Where(tbl => tbl.RegionalOfficePK == t.RegionalOfficeFK).DefaultIfEmpty()
                                               from t4 in substationsTable.Where(tbl => tbl.SubstationPK == t.SubstationFK).DefaultIfEmpty()
                                               from t5 in commercialCourtsTable.Where(tbl => tbl.CommercialCourtPK == t.CommercialCourtFK).DefaultIfEmpty()
                                               from t6 in taxesTable.Where(tbl => tbl.TaxPK == t.TaxFK).DefaultIfEmpty()
                                               from t7 in countriesTable.Where(tbl => tbl.CountryPK == t.CountryFK).DefaultIfEmpty()
                                               from t8 in countiesTable.Where(tbl => tbl.CountyPK == t.CountyFK).DefaultIfEmpty()
                                               from t9 in cityCommunityTable.Where(tbl => tbl.CityCommunityPK == t.CityCommunityFK).DefaultIfEmpty()
                                               from t10 in postalOfficeTable.Where(tbl => tbl.PostalOfficePK == t.PostalOfficeFK).DefaultIfEmpty()
                                               from t11 in placesTable.Where(tbl => tbl.PlacePK == t.PlaceFK).DefaultIfEmpty()
                                               from t12 in physicalEntitiesTable.Where(tbl => tbl.PhysicalEntityPK == t.ReferentFK).DefaultIfEmpty()

                                               select new LegalEntityAuditView
                                                {
                                                    LegalEntityPK               = t.LegalEntityPK,
                                                    Name                        = t.Name != null ? t.Name : null,
                                                    ShortName                   = t.ShortName != null ? t.ShortName : null,
                                                    OIB                         = t.OIB != null ? t.OIB : null,
                                                    MB                          = t.MB != null ? t.MB : null,
                                                    MBS                         = t.MBS != null ? t.MBS : null,
                                                    ActivityFK                  = t.ActivityFK != null ? t.ActivityFK : null,
                                                    ActivityDescription         = t.ActivityDescription != null ? t.ActivityDescription : null,
                                                    FormFK                      = t.FormFK != null ? t.FormFK : null,
                                                    FundamentalCapital          = t.FundamentalCapital != null ? t.FundamentalCapital : null,

                                                    CountryFK                   = t.CountryFK != null ? t.CountryFK : null,
                                                    CountyFK                    = t.CountyFK != null ? t.CountyFK : null,
                                                    CityCommunityFK             = t.CityCommunityFK != null ? t.CityCommunityFK : null,
                                                    PostalOfficeFK              = t.PostalOfficeFK != null ? t.PostalOfficeFK : null,
                                                    PlaceFK                     = t.PlaceFK != null ? t.PlaceFK : null,
                                                    Place                       = t.Place != null ? t.Place : null,
                                                    StreetName                  = t.StreetName != null ? t.StreetName : null,

                                                    RegionalOfficeFK            = t.RegionalOfficeFK != null ? t.RegionalOfficeFK : null,
                                                    SubstationFK                = t.SubstationFK != null ? t.SubstationFK : null,
                                                    ReferentFK                  = t.ReferentFK != null ? t.ReferentFK : null,
                                                    DateOfRegistration          = t.DateOfRegistration != null ? t.DateOfRegistration : null,
                                                    CommercialCourtFK           = t.CommercialCourtFK != null ? t.CommercialCourtFK : null,
                                                    TaxFK                       = t.TaxFK != null ? t.TaxFK : null,

                                                    Phone                       = t.Phone != null ? t.Phone : null,
                                                    Fax                         = t.Fax != null ? t.Fax : null,
                                                    Mobile                      = t.Mobile != null ? t.Mobile : null,
                                                    EMail                       = t.EMail != null ? t.EMail : null,
                                                    FirstContactDate            = t.FirstContactDate != null ? t.FirstContactDate : null,
                                                    TouristOffice               = t.TouristOffice != null ? t.TouristOffice : null,
                                                    TouristOfficeDescription    = t.TouristOfficeDescription != null ? t.TouristOfficeDescription : null,
                                                    MonumentAnnuity             = t.MonumentAnnuity != null ? t.MonumentAnnuity : null,
                                                    MonumentAnnuityDescription  = t.MonumentAnnuityDescription != null ? t.MonumentAnnuityDescription : null,
                                                    NumberOfEmployees           = t.NumberOfEmployees != null ? t.NumberOfEmployees : null,
                                                    MIORRegistrationNumber      = t.MIORRegistrationNumber != null ? t.MIORRegistrationNumber : null,
                                                    HZZOObligationNumber        = t.HZZOObligationNumber != null ? t.HZZOObligationNumber : null,
                                                    HZZOBussinesEntityCode      = t.HZZOBussinesEntityCode != null ? t.HZZOBussinesEntityCode : null,
                                                    ChangeDate                = t.ChangeDate != null ? t.ChangeDate : null,

                                                    Owner                       = t.Owner != null ? t.Owner : null,
                                                    LegalEntity                 = t.LegalEntity != null ? t.LegalEntity : null,
                                                    Active                      = t.Active != null ? t.Active : null,
                                                    Deleted                     = t.Deleted != null ? t.Deleted : null,

                                                    FormName                    = t1 != null && t1.Name != null ? t1.Name : null,
                                                    ActivityName                = t2 != null && t2.Name != null ? t2.Name : null,
                                                    RegionalOfficeName          = t3 != null && t3.Name != null ? t3.Name : null,
                                                    SubstationName              = t4 != null && t4.Name != null ? t4.Name : null,
                                                    ReferentName                = t12 != null && t12.Firstname != null && t12.Lastname != null ? t12.Firstname + " " + t12.Lastname : null,
                                                    CommercialCourtName         = t5 != null && t5.Name != null ? t5.Name : null,
                                                    TaxName                     = t6 != null && t6.Name != null ? t6.Name : null,
                                                    CountryName                 = t7 != null && t7.Name != null ? t7.Name : null,
                                                    CountyName                  = t8 != null && t8.Name != null ? t8.Name : null,
                                                    CityCommunityName           = t9 != null && t9.Name != null ? t9.Name : null,
                                                    PostalOfficeName            = t10 != null && t10.Name != null ? t10.Name : null,
                                                    PlaceName                   = t11 != null && t11.Name != null ? t11.Name : null,
                                                }).OrderBy(c => c.ChangeDate).ToList();

            return legalEntityAuditView;
        }

    }
}
