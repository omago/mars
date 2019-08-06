using System;
using System.Collections.Generic;
using System.Linq;
using Mateus.Model.EFModel;
using System.Data.Objects;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.BussinesLogic.Views.LegalEntityBranchModel;
using Mateus.Model.BussinesLogic.Support.ExtensionMethods;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityBranchAuditModel
{
    public class LegalEntityBranchAuditView
    {
        public int LegalEntityBranchPK { get; set; }

        public string Name { get; set; }

        public int? LegalEntityFK { get; set; }

        public int? CountryFK { get; set; }

        public int? CityCommunityFK { get; set; }

        public int? CountyFK { get; set; }

        public int? PlaceFK { get; set; }

        public string StreetName { get; set; }

        public int? PostalOfficeFK { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string EMail { get; set; }

        public string CountryName { get; set; }
        public string CountyName { get; set; }
        public string PlaceName { get; set; }
        public string CityCommunityName { get; set; }
        public string PostalOfficeName { get; set; }

        public DateTime? ChangeDate { get; set; }

        public bool? Deleted { get; set; }

        public static List<List<LegalEntityBranchAuditView>> GetLegalEntityBranchesAuditView(ObjectContext context, int legalEntityFK) 
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(context);
            ILegalEntityBranchesRepository legalEntityLegalEntityBranchesRepository = new LegalEntityBranchesRepository(context); 

            // get all legalEntity legalEntityBranches
            List<LegalEntityBranchView> legalEntityBranchesList = LegalEntityBranchView.GetLegalEntityBranchView(legalEntityLegalEntityBranchesRepository.GetAll(), legalEntitiesRepository.GetValid())
                                                            .Where(c => c.LegalEntityFK == legalEntityFK)
                                                            .ToList();

            List<List<LegalEntityBranchAuditView>> LegalEntityBranchesListList = new List<List<LegalEntityBranchAuditView>>();

            foreach(LegalEntityBranchView legalEntityBranch in legalEntityBranchesList)
            {
                LegalEntityBranchesListList.Add(LegalEntityBranchAuditView.GetLegalEntityBranchAuditView(context, legalEntityBranch.LegalEntityBranchPK));
            }

            return LegalEntityBranchesListList;
        }


        public static List<LegalEntityBranchAuditView> GetLegalEntityBranchAuditView(ObjectContext context, int relatedEntityPK) 
        {
            IAuditingDetailsRepository auditingDetailsRepository = new AuditingDetailsRepository(context); 
            IAuditingMasterRepository auditingMasterRepository = new AuditingMasterRepository(context);

            var sessionTokens = (from am in auditingMasterRepository.GetAll().Where(c => c.TableName == "LegalEntityBranches")
                                    where am.RelatedEntityPK == relatedEntityPK
                                    select new { 
                                    AuditingMasterPK = am.AuditingMasterPK, 
                                    RelatedEntityPK = am.RelatedEntityPK, 
                                    SessionToken = am.SessionToken 
                                }).ToList();

            List<LegalEntityBranchAuditView> legalEntityLegalEntityBranchAuditViewList = new List<LegalEntityBranchAuditView>();

            foreach (var item in sessionTokens) 
            {
                var record = auditingDetailsRepository.GetAuditingDetailByAuditingMasterPK(item.AuditingMasterPK).ToList();

                LegalEntityBranchAuditView legalEntityLegalEntityBranchAuditView = new LegalEntityBranchAuditView();

                legalEntityLegalEntityBranchAuditView.Name = record.checkString("Name");

                legalEntityLegalEntityBranchAuditView.CountryFK = record.checkInteger("CountryFK");
                legalEntityLegalEntityBranchAuditView.CountyFK = record.checkInteger("CountyFK");
                legalEntityLegalEntityBranchAuditView.CityCommunityFK = record.checkInteger("CityCommunityFK");
                legalEntityLegalEntityBranchAuditView.PostalOfficeFK = record.checkInteger("PostalOfficeFK");
                legalEntityLegalEntityBranchAuditView.PlaceFK = record.checkInteger("PlaceFK");
                legalEntityLegalEntityBranchAuditView.StreetName = record.checkString("StreetName");

                legalEntityLegalEntityBranchAuditView.Phone = record.checkString("Phone");
                legalEntityLegalEntityBranchAuditView.Fax = record.checkString("Fax");
                legalEntityLegalEntityBranchAuditView.Mobile = record.checkString("Mobile");
                legalEntityLegalEntityBranchAuditView.EMail = record.checkString("EMail");

                legalEntityLegalEntityBranchAuditView.ChangeDate = record.checkDate("ChangeDate");
                legalEntityLegalEntityBranchAuditView.Deleted = record.checkBoolean("Deleted");

                legalEntityLegalEntityBranchAuditViewList.Add(legalEntityLegalEntityBranchAuditView);
            }

            ICountriesRepository countriesRepository = new CountriesRepository(context);
            IQueryable<Country> countriesTable = countriesRepository.GetValid();

            ICountiesRepository countiesRepository = new CountiesRepository(context);
            IQueryable<County> countiesTable = countiesRepository.GetValid();

            ICitiesCommunitiesRepository citiesCommunitiesRepository = new CitiesCommunitiesRepository(context);
            IQueryable<CityCommunity> citiesCommunitiesTable = citiesCommunitiesRepository.GetValid();

            IPlacesRepository placesRepository = new PlacesRepository(context);
            IQueryable<Place> placesTable = placesRepository.GetValid();

            IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(context);
            IQueryable<PostalOffice> postalOfficesTable = postalOfficesRepository.GetValid();

            List<LegalEntityBranchAuditView> legalEntityLegalEntityBranch  =  
                                            ( from t in legalEntityLegalEntityBranchAuditViewList
                                            from t1 in countriesTable.Where(tbl => tbl.CountryPK == t.CountryFK).DefaultIfEmpty()
                                            from t2 in countiesTable.Where(tbl => tbl.CountyPK == t.CountyFK).DefaultIfEmpty()
                                            from t3 in citiesCommunitiesTable.Where(tbl => tbl.CityCommunityPK == t.CityCommunityFK).DefaultIfEmpty()
                                            from t4 in placesTable.Where(tbl => tbl.PlacePK == t.PlaceFK).DefaultIfEmpty()
                                            from t5 in postalOfficesTable.Where(tbl => tbl.PostalOfficePK == t.PostalOfficeFK).DefaultIfEmpty()
                                            where t.ChangeDate != null
                                            select new LegalEntityBranchAuditView
                                            {
                                                LegalEntityBranchPK = t.LegalEntityBranchPK,
                                                Name                = t.Name,

                                                CountryName         = t1 != null && t1.Name != null ? t1.Name : null,
                                                CountyName          = t2 != null && t2.Name != null ? t2.Name : null,
                                                CityCommunityName   = t3 != null && t3.Name != null ? t3.Name : null,
                                                PlaceName           = t4 != null && t4.Name != null ? t4.Name : null,
                                                PostalOfficeName    = t5 != null && t5.Name != null ? t5.Name : null,
                                                StreetName          = t.StreetName != null ? t.StreetName : null,

                                                Phone               = t.Phone != null ? t.Phone : null,
                                                Fax                 = t.Fax != null ? t.Fax : null,
                                                Mobile              = t.Mobile != null ? t.Mobile : null,
                                                EMail               = t.EMail != null ? t.EMail : null,

                                                ChangeDate        = t.ChangeDate != null ? t.ChangeDate : null,
                                                Deleted             = t.Deleted != null ? t.Deleted : null,
                                            }).OrderBy(c => c.ChangeDate).ToList();

            return legalEntityLegalEntityBranch;
        }
    }
}
