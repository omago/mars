using System;
using System.Collections.Generic;
using System.Linq;
using Mateus.Model.EFModel;
using System.Data.Objects;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.BussinesLogic.Support.ExtensionMethods;

namespace Mateus.Model.BussinesLogic.Views.PhysicalEntityAuditModel
{
    public class PhysicalEntityAuditView
    {
        public int PhysicalEntityPK { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Gender { get; set; }

        public string OIB { get; set; }
        public string JMBG { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? CitizenshipFK { get; set; }
        public int? BirthCountryFK { get; set; }
        public int? BirthCountyFK { get; set; }
        public int? BirthCityCommunityFK { get; set; }
        public string BirthPlace { get; set; }

        public int? ResidenceCountryFK { get; set; }
        public int? ResidenceCountyFK { get; set; }
        public int? ResidenceCityCommunityFK { get; set; }
        public int? ResidencePostalOfficeFK { get; set; }
        public int? ResidencePlaceFK { get; set; }
        public string ResidencePlace { get; set; }
        public string ResidenceStreetName { get; set; }

        public string IdentityCardNumber { get; set; }
        public DateTime? IdentityCardDateOfIssue { get; set; }
        public int? IdentityCardRegionalOfficeFK { get; set; }
        public DateTime? IdentityCardDateTillValid { get; set; }
        
        public string PassportNumber { get; set; }
        public DateTime? PassportDateOfIssue { get; set; }
        public DateTime? PassportDateTillValid { get; set; }
        public int? PassportCountryOfIssueFK { get; set; }
        public string PassportPlaceOfIssue { get; set; }

        public int? ReferentRegionalOfficeFK { get; set; }
        public int? ReferentSubstationFK { get; set; }

        public DateTime? ChangeDate { get; set; }

        public bool? LegalRepresentative { get; set; }
        public bool? Owner { get; set; }
        public bool? Referent { get; set; }
        public bool? Deleted { get; set; }
        
        public string Citizenship { get; set; }

        public string BirthCountryName { get; set; }
        public string BirthCountyName { get; set; }
        public string BirthCityCommunityName { get; set; }

        public string ResidenceCountryName { get; set; }
        public string ResidenceCountyName { get; set; }
        public string ResidenceCityCommunityName { get; set; }
        public string ResidencePostalOfficeName { get; set; }
        public string ResidencePlaceName { get; set; }

        public string PassportCountryOfIssueName { get; set; }
        public string IdentityCardRegionalOfficeName { get; set; }

        public string ReferentRegionalOfficeName { get; set; }
        public string ReferentSubstationName { get; set; }

        public static List<PhysicalEntityAuditView> GetPhysicalEntityAuditView(ObjectContext context, int relatedEntityPK) 
        {
            IAuditingDetailsRepository auditingDetailsRepository = new AuditingDetailsRepository(context); 
            IAuditingMasterRepository auditingMasterRepository = new AuditingMasterRepository(context);

            List<PhysicalEntityAuditView> physicalEntityAuditViewList = new List<PhysicalEntityAuditView>();

            var sessionTokens = (from am in auditingMasterRepository.GetAll().Where(c => c.TableName == "PhysicalEntities")
                                where am.RelatedEntityPK == relatedEntityPK
                                select new { 
                                    auditingMasterPK = am.AuditingMasterPK, 
                                    sessionToken = am.SessionToken 
                                }).ToList();

            foreach (var item in sessionTokens) 
            {
                List<AuditingDetail> record = auditingDetailsRepository.GetAuditingDetailByAuditingMasterPK(item.auditingMasterPK).ToList();

                PhysicalEntityAuditView peav = new PhysicalEntityAuditView();

                peav.Firstname = record.checkString("Firstname");
                peav.Lastname = record.checkString("Lastname");

                peav.Gender = record.checkString("Gender");

                peav.OIB = record.checkString("OIB");
                peav.JMBG = record.checkString("JMBG");

                peav.DateOfBirth = record.checkDate("DateOfBirth");

                peav.CitizenshipFK = record.checkInteger("CitizenshipFK");
                peav.BirthCountryFK = record.checkInteger("BirthCountryFK");
                peav.BirthCountyFK = record.checkInteger("BirthCountyFK");
                peav.BirthCityCommunityFK = record.checkInteger("BirthCityCommunityFK");
                peav.BirthPlace = record.checkString("BirthPlace");

                peav.ResidenceCountryFK = record.checkInteger("ResidenceCountryFK");
                peav.ResidenceCountyFK = record.checkInteger("ResidenceCountyFK");
                peav.ResidenceCityCommunityFK = record.checkInteger("ResidenceCityCommunityFK");
                peav.ResidencePostalOfficeFK = record.checkInteger("ResidencePostalOfficeFK");
                peav.ResidencePlaceFK = record.checkInteger("ResidencePlaceFK");
                peav.ResidencePlace = record.checkString("ResidencePlace");
                peav.ResidenceStreetName = record.checkString("ResidenceStreetName");

                peav.IdentityCardNumber = record.checkString("IdentityCardNumber");
                peav.IdentityCardDateOfIssue = record.checkDate("IdentityCardDateOfIssue");
                peav.IdentityCardRegionalOfficeFK = record.checkInteger("IdentityCardRegionalOfficeFK");
                peav.IdentityCardDateTillValid = record.checkDate("IdentityCardDateTillValid");

                peav.PassportNumber = record.checkString("PassportNumber");
                peav.PassportDateOfIssue = record.checkDate("PassportDateOfIssue");
                peav.PassportDateTillValid = record.checkDate("PassportDateTillValid");
                peav.PassportCountryOfIssueFK = record.checkInteger("PassportCountryOfIssueFK");
                peav.PassportPlaceOfIssue = record.checkString("PassportPlaceOfIssue");

                peav.ReferentRegionalOfficeFK = record.checkInteger("ReferentRegionalOfficeFK");
                peav.ReferentSubstationFK = record.checkInteger("ReferentSubstationFK");

                peav.ChangeDate = record.checkDate("ChangeDate");

                peav.LegalRepresentative = record.checkBoolean("LegalRepresentative");
                peav.Owner = record.checkBoolean("Owner");
                peav.Referent = record.checkBoolean("Referent");

                physicalEntityAuditViewList.Add(peav);
            }

            // Connect all foreign keys and return data collection as List<PhysicalEntityAuditView>
            ICountriesRepository countriesRepository = new CountriesRepository(context); 
            IQueryable<Country> countryTable = countriesRepository.GetValid();

            ICountiesRepository countiesRepository = new CountiesRepository(context);
            IQueryable<County> countyTable = countiesRepository.GetValid();

            ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(context);
            IQueryable<CityCommunity> cityCommunityTable = citiesCommunitiesRepository.GetValid();

            IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(context);
            IQueryable<PostalOffice> postalOfficeTable = postalOfficesRepository.GetValid();

            IPlacesRepository placesRepository = new PlacesRepository(context);
            IQueryable<Place> placeTable = placesRepository.GetValid();

            IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(context);
            IQueryable<RegionalOffice> regionalOfficeTable = regionalOfficesRepository.GetValid();

            ISubstationsRepository substationsRepository = new SubstationsRepository(context);
            IQueryable<Substation> substationTable = substationsRepository.GetValid();

            List<PhysicalEntityAuditView> physicalEntityAuditView =  
                                              (from t in physicalEntityAuditViewList

                                                from t1 in countryTable.Where(tbl => tbl.CountryPK == t.CitizenshipFK).DefaultIfEmpty()
                                                from t2 in countryTable.Where(tbl => tbl.CountryPK == t.CitizenshipFK).DefaultIfEmpty()
                                                from t3 in countyTable.Where(tbl => tbl.CountyPK == t.BirthCountyFK).DefaultIfEmpty()
                                                from t4 in cityCommunityTable.Where(tbl => tbl.CityCommunityPK == t.BirthCityCommunityFK).DefaultIfEmpty()

                                                from t5 in countryTable.Where(tbl => tbl.CountryPK == t.ResidenceCountryFK).DefaultIfEmpty()
                                                from t6 in countyTable.Where(tbl => tbl.CountyPK == t.ResidenceCountyFK).DefaultIfEmpty()
                                                from t7 in cityCommunityTable.Where(tbl => tbl.CityCommunityPK == t.ResidenceCityCommunityFK).DefaultIfEmpty()
                                                from t8 in postalOfficeTable.Where(tbl => tbl.PostalOfficePK == t.ResidencePostalOfficeFK).DefaultIfEmpty()
                                                from t9 in placeTable.Where(tbl => tbl.PlacePK == t.ResidencePlaceFK).DefaultIfEmpty()

                                                from t10 in countryTable.Where(tbl => tbl.CountryPK == t.PassportCountryOfIssueFK).DefaultIfEmpty()
                                                from t11 in regionalOfficeTable.Where(tbl => tbl.RegionalOfficePK == t.IdentityCardRegionalOfficeFK).DefaultIfEmpty()

                                                from t12 in regionalOfficeTable.Where(tbl => tbl.RegionalOfficePK == t.ReferentRegionalOfficeFK).DefaultIfEmpty()
                                                from t13 in substationTable.Where(tbl => tbl.SubstationPK == t.ReferentSubstationFK).DefaultIfEmpty()
                                                select new PhysicalEntityAuditView
                                                {
                                                    PhysicalEntityPK                = t.PhysicalEntityPK,
                                                    Firstname                       = t.Firstname != null ? t.Firstname : null,
                                                    Lastname                        = t.Lastname != null ? t.Lastname : null,

                                                    Gender                          = t.Gender != null ? t.Gender : null,
                                                    OIB                             = t.OIB != null ? t.OIB : null,
                                                    JMBG                            = t.JMBG != null ? t.JMBG : null,

                                                    DateOfBirth                     = t.DateOfBirth != null ? t.DateOfBirth : null,

                                                    CitizenshipFK                   = t.CitizenshipFK != null ? t.CitizenshipFK : null,
                                                    BirthCountryFK                  = t.BirthCountryFK != null ? t.BirthCountryFK : null,
                                                    BirthCountyFK                   = t.BirthCountyFK != null ? t.BirthCountyFK : null,
                                                    BirthCityCommunityFK            = t.BirthCityCommunityFK != null ? t.BirthCityCommunityFK : null,
                                                    BirthPlace                      = t.BirthPlace != null ? t.BirthPlace : null,

                                                    ResidenceCountryFK              = t.ResidenceCountryFK != null ? t.ResidenceCountryFK : null,
                                                    ResidenceCountyFK               = t.ResidenceCountyFK != null ? t.ResidenceCountyFK : null,
                                                    ResidenceCityCommunityFK        = t.ResidenceCityCommunityFK != null ? t.ResidenceCityCommunityFK : null,
                                                    ResidencePostalOfficeFK         = t.ResidencePostalOfficeFK != null ? t.ResidencePostalOfficeFK : null,
                                                    ResidencePlaceFK                = t.ResidencePlaceFK != null ? t.ResidencePlaceFK : null,
                                                    ResidencePlace                  = t.ResidencePlace != null ? t.ResidencePlace : null,
                                                    ResidenceStreetName             = t.ResidenceStreetName != null ? t.ResidenceStreetName : null,

                                                    IdentityCardNumber              = t.IdentityCardNumber != null ? t.IdentityCardNumber : null,
                                                    IdentityCardDateOfIssue         = t.IdentityCardDateOfIssue != null ? t.IdentityCardDateOfIssue : null,
                                                    IdentityCardRegionalOfficeFK    = t.IdentityCardRegionalOfficeFK != null ? t.IdentityCardRegionalOfficeFK : null,
                                                    IdentityCardDateTillValid       = t.IdentityCardDateTillValid != null ? t.IdentityCardDateTillValid : null,

                                                    PassportNumber                  = t.PassportNumber != null ? t.PassportNumber : null,
                                                    PassportDateOfIssue             = t.PassportDateOfIssue != null ? t.PassportDateOfIssue : null,
                                                    PassportDateTillValid           = t.PassportDateTillValid != null ? t.PassportDateTillValid : null,
                                                    PassportCountryOfIssueFK        = t.PassportCountryOfIssueFK != null ? t.PassportCountryOfIssueFK : null,
                                                    PassportPlaceOfIssue            = t.PassportPlaceOfIssue != null ? t.PassportPlaceOfIssue : null,

                                                    ReferentRegionalOfficeFK        = t.ReferentRegionalOfficeFK != null ? t.ReferentRegionalOfficeFK : null,
                                                    ReferentSubstationFK            = t.ReferentSubstationFK != null ? t.ReferentSubstationFK : null,

                                                    ChangeDate                    = t.ChangeDate != null ? t.ChangeDate : null,

                                                    LegalRepresentative             = t.LegalRepresentative != null ? t.LegalRepresentative : null,
                                                    Owner                           = t.Owner != null ? t.Owner : null,
                                                    Referent                        = t.Referent != null ? t.Referent : null,

                                                    Deleted                         = t.Deleted != null ? t.Deleted : null,

                                                    Citizenship                     = t1 != null && t1.Citizenship != null ? t1.Citizenship : null,
                                                    BirthCountryName                = t2 != null && t2.Name != null ? t2.Name : null,
                                                    BirthCountyName                 = t3 != null && t3.Name != null ? t3.Name : null,
                                                    BirthCityCommunityName          = t4 != null && t4.Name != null ? t4.Name : null,

                                                    ResidenceCountryName            = t5 != null && t5.Name != null ? t5.Name : null,
                                                    ResidenceCountyName             = t6 != null && t6.Name != null ? t6.Name : null,
                                                    ResidenceCityCommunityName      = t7 != null && t7.Name != null ? t7.Name : null,
                                                    ResidencePostalOfficeName       = t8 != null && t8.Name != null ? t8.Name : null,
                                                    ResidencePlaceName              = t9 != null && t9.Name != null ? t9.Name : null,

                                                    PassportCountryOfIssueName      = t10 != null && t10.Name != null ? t10.Name : null,
                                                    IdentityCardRegionalOfficeName  = t11 != null && t11.Name != null ? t11.Name : null,

                                                    ReferentRegionalOfficeName      = t12 != null && t12.Name != null ? t12.Name : null,
                                                    ReferentSubstationName          = t13 != null && t13.Name != null ? t13.Name : null,
                                                }).OrderBy(c => c.ChangeDate).ToList();

            return physicalEntityAuditView;
        }

        

    }

}
