using System;
using System.Collections.Generic;
using System.Linq;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;
using Mateus.Model.BussinesLogic.Views.LegalEntityOwnerModel;
using Mateus.Model.BussinesLogic.Support.ExtensionMethods;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityOwnerAuditModel
{
    public class LegalEntityOwnerAuditView
    {
        public int LegalEntityOwnerPK { get; set; }

        public int? LegalEntityFK { get; set; }

        public int? OwnerFK { get; set; }
        public string OwnerType { get; set; }

        public decimal? BusinessShareAmount { get; set; }
        public decimal? PaidBussinesShareAmount { get; set; }
        public decimal? NominalBussinesShareAmount { get; set; }

        public int? AdditionalFactorFK { get; set; }
        public int? FulfilledFactorFK { get; set; }
        public int? BussinesShareBurdenFK { get; set; }
        public int? ChangeTypeFK { get; set; }

        public int? NumberOfVotes { get; set; }

        public DateTime? EntryDate { get; set; }
        public DateTime? ChangeDate { get; set; }

        public bool? Deleted { get; set; }

        public string LegalEntityName { get; set; }
        public string OwnerName { get; set; }
        public string AdditionalFactorName { get; set; }
        public string FulfilledFactorName { get; set; }
        public string BussinesShareBurdenName { get; set; }
        public string ChangeTypeName { get; set; }

        public static List<List<LegalEntityOwnerAuditView>> GetLegalEntityOwnersAuditView(ObjectContext context, int legalEntityFK) 
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(context);
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(context);
            ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(context);

            // get all legalEntity branches
            List<LegalEntityOwnerView> legalEntityOwners = LegalEntityOwnerView.GetLegalEntityOwnerView(legalEntityOwnersRepository.GetAll(), physicalEntitiesRepository.GetValid(), legalEntitiesRepository.GetValid())
                                                            .Where(c => c.LegalEntityFK == legalEntityFK)
                                                            .ToList();

            List<List<LegalEntityOwnerAuditView>> legalEntityOwnersListList = new List<List<LegalEntityOwnerAuditView>>();

            foreach(LegalEntityOwnerView legalEntityOwner in legalEntityOwners)
            {
                legalEntityOwnersListList.Add(LegalEntityOwnerAuditView.GetLegalEntityOwnerAuditView(context, legalEntityOwner.LegalEntityOwnerPK));
            }

            return legalEntityOwnersListList;
        }


        public static List<LegalEntityOwnerAuditView> GetLegalEntityOwnerAuditView(ObjectContext context, int relatedEntityPK) 
        {
            IAuditingDetailsRepository auditingDetailsRepository = new AuditingDetailsRepository(context); 
            IAuditingMasterRepository auditingMasterRepository = new AuditingMasterRepository(context);

            var sessionTokens = (from am in auditingMasterRepository.GetAll().Where(c => c.TableName == "LegalEntityOwners")
                                    where am.RelatedEntityPK == relatedEntityPK
                                    select new { 
                                    AuditingMasterPK = am.AuditingMasterPK, 
                                    RelatedEntityPK = am.RelatedEntityPK, 
                                    SessionToken = am.SessionToken 
                                }).ToList();

            List<LegalEntityOwnerAuditView> legalEntityOwnerAuditViewList = new List<LegalEntityOwnerAuditView>();

            foreach (var item in sessionTokens) 
            {
                var record = auditingDetailsRepository.GetAuditingDetailByAuditingMasterPK(item.AuditingMasterPK).ToList();

                LegalEntityOwnerAuditView legalEntityOwnerAuditView = new LegalEntityOwnerAuditView();

                legalEntityOwnerAuditView.LegalEntityFK = record.checkInteger("LegalEntityFK");
                legalEntityOwnerAuditView.OwnerFK = record.checkInteger("OwnerFK");
                legalEntityOwnerAuditView.OwnerType = record.checkString("OwnerType");

                legalEntityOwnerAuditView.BusinessShareAmount = record.checkDecimal("BusinessShareAmount");
                legalEntityOwnerAuditView.PaidBussinesShareAmount = record.checkDecimal("PaidBussinesShareAmount");
                legalEntityOwnerAuditView.NominalBussinesShareAmount = record.checkDecimal("NominalBussinesShareAmount");

                legalEntityOwnerAuditView.AdditionalFactorFK = record.checkInteger("AdditionalFactorFK");
                legalEntityOwnerAuditView.FulfilledFactorFK = record.checkInteger("FulfilledFactorFK");
                legalEntityOwnerAuditView.BussinesShareBurdenFK = record.checkInteger("BussinesShareBurdenFK");
                legalEntityOwnerAuditView.ChangeTypeFK = record.checkInteger("ChangeTypeFK");
                legalEntityOwnerAuditView.NumberOfVotes = record.checkInteger("NumberOfVotes");

                legalEntityOwnerAuditView.EntryDate = record.checkDate("EntryDate");
                legalEntityOwnerAuditView.ChangeDate = record.checkDate("ChangeDate");

                legalEntityOwnerAuditView.Deleted = record.checkBoolean("Deleted");

                legalEntityOwnerAuditViewList.Add(legalEntityOwnerAuditView);
            }

            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(context);
            IQueryable<LegalEntity> legalEntitiesTable = legalEntitiesRepository.GetValid();

            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(context);
            IQueryable<PhysicalEntity> physicalEntitiesTable = physicalEntitiesRepository.GetValid();

            IAdditionalFactorsRepository additionalFactorsRepository = new AdditionalFactorsRepository(context);
            IQueryable<AdditionalFactor> additionalFactorsTable = additionalFactorsRepository.GetValid();

            IFulfilledFactorsRepository fulfilledFactorsRepository = new FulfilledFactorsRepository(context);
            IQueryable<FulfilledFactor> fulfilledFactorsTable = fulfilledFactorsRepository.GetValid();

            IBussinesShareBurdensRepository bussinesShareBurdensRepository = new BussinesShareBurdensRepository(context);
            IQueryable<BussinesShareBurden> bussinesShareBurdensTable = bussinesShareBurdensRepository.GetValid();

            IChangeTypesRepository changeTypesRepository = new ChangeTypesRepository(context);
            IQueryable<ChangeType> changeTypesTable = changeTypesRepository.GetValid();

            IQueryable<LegalEntityOwnerAuditView> legalEntityOwnersLE =  
                                            ( from t in legalEntityOwnerAuditViewList
                                            from t1 in legalEntitiesTable.Where(tbl => tbl.LegalEntityPK == t.OwnerFK).DefaultIfEmpty()
                                            from t2 in additionalFactorsTable.Where(tbl => tbl.AdditionalFactorPK == t.AdditionalFactorFK).DefaultIfEmpty()
                                            from t3 in fulfilledFactorsTable.Where(tbl => tbl.FulfilledFactorPK == t.FulfilledFactorFK).DefaultIfEmpty()
                                            from t4 in bussinesShareBurdensTable.Where(tbl => tbl.BussinesShareBurdenPK == t.BussinesShareBurdenFK).DefaultIfEmpty()
                                            from t5 in changeTypesTable.Where(tbl => tbl.ChangeTypePK == t.ChangeTypeFK).DefaultIfEmpty()
                                            
                                            where t.ChangeDate != null && t.OwnerType != null && t.OwnerType.Contains("le")
                                            select new LegalEntityOwnerAuditView
                                            {
                                                LegalEntityOwnerPK          = t.LegalEntityOwnerPK,

                                                OwnerName                   = t1 != null && t1.Name != null ? t1.Name : null,
                                                AdditionalFactorName        = t2 != null && t2.Name != null ? t2.Name : null,
                                                FulfilledFactorName         = t3 != null && t3.Name != null ? t3.Name : null,
                                                BussinesShareBurdenName     = t4 != null && t4.Name != null ? t4.Name : null,
                                                ChangeTypeName              = t5 != null && t5.Name != null ? t5.Name : null,

                                                BusinessShareAmount         = t.BusinessShareAmount != null ? t.BusinessShareAmount : null,
                                                PaidBussinesShareAmount     = t.PaidBussinesShareAmount != null ? t.PaidBussinesShareAmount : null,
                                                NominalBussinesShareAmount  = t.NominalBussinesShareAmount != null ? t.NominalBussinesShareAmount : null,

                                                NumberOfVotes               = t.NumberOfVotes != null ? t.NumberOfVotes : null,

                                                EntryDate                   = t.EntryDate != null ? t.EntryDate : null,
                                                ChangeDate                = t.ChangeDate != null ? t.ChangeDate : null,
                                                Deleted                     = t.Deleted != null ? t.Deleted : null,
                                            }).AsQueryable<LegalEntityOwnerAuditView>();


            IQueryable<LegalEntityOwnerAuditView> legalEntityOwnersPE =  
                                            ( from t in legalEntityOwnerAuditViewList
                                            from t1 in physicalEntitiesTable.Where(tbl => tbl.PhysicalEntityPK == t.OwnerFK).DefaultIfEmpty()
                                            from t2 in additionalFactorsTable.Where(tbl => tbl.AdditionalFactorPK == t.AdditionalFactorFK).DefaultIfEmpty()
                                            from t3 in fulfilledFactorsTable.Where(tbl => tbl.FulfilledFactorPK == t.FulfilledFactorFK).DefaultIfEmpty()
                                            from t4 in bussinesShareBurdensTable.Where(tbl => tbl.BussinesShareBurdenPK == t.BussinesShareBurdenFK).DefaultIfEmpty()
                                            from t5 in changeTypesTable.Where(tbl => tbl.ChangeTypePK == t.ChangeTypeFK).DefaultIfEmpty()
                                            
                                            where t.ChangeDate != null && t.OwnerType != null && t.OwnerType.Contains("pe")
                                            select new LegalEntityOwnerAuditView
                                            {
                                                LegalEntityOwnerPK          = t.LegalEntityOwnerPK,

                                                OwnerName                   = t1 != null && t1.Firstname != null && t1.Lastname != null ? t1.Firstname + " " + t1.Lastname : null,
                                                AdditionalFactorName        = t2 != null && t2.Name != null ? t2.Name : null,
                                                FulfilledFactorName         = t3 != null && t3.Name != null ? t3.Name : null,
                                                BussinesShareBurdenName     = t4 != null && t4.Name != null ? t4.Name : null,
                                                ChangeTypeName              = t5 != null && t5.Name != null ? t5.Name : null,

                                                BusinessShareAmount         = t.BusinessShareAmount != null ? t.BusinessShareAmount : null,
                                                PaidBussinesShareAmount     = t.PaidBussinesShareAmount != null ? t.PaidBussinesShareAmount : null,
                                                NominalBussinesShareAmount  = t.NominalBussinesShareAmount != null ? t.NominalBussinesShareAmount : null,

                                                NumberOfVotes               = t.NumberOfVotes != null ? t.NumberOfVotes : null,

                                                EntryDate                   = t.EntryDate != null ? t.EntryDate : null,

                                                ChangeDate                = t.ChangeDate != null ? t.ChangeDate : null,
                                                Deleted                     = t.Deleted != null ? t.Deleted : null,
                                            }).AsQueryable<LegalEntityOwnerAuditView>();


            List<LegalEntityOwnerAuditView> legalEntityOwners = legalEntityOwnersLE.Union(legalEntityOwnersPE).OrderBy(c => c.ChangeDate).ToList();

            return legalEntityOwners;
        }
    }
}
