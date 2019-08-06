using System;
using System.Collections.Generic;
using System.Linq;
using Mateus.Model.EFModel;
using System.Data.Objects;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.BussinesLogic.Views.LegalEntityBankModel;
using Mateus.Model.BussinesLogic.Support.ExtensionMethods;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityBankAuditModel
{
    public class LegalEntityBankAuditView
    {
        public int LegalEntityBankPK { get; set; }

        public int? LegalEntityFK { get; set; }

        public int? BankFK { get; set; }

        public string Iban { get; set; }

        public DateTime? ChangeDate { get; set; }

        public bool? Deleted { get; set; }

        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIban { get; set; }
        public string LegalEntityName { get; set; }

        public static List<List<LegalEntityBankAuditView>> GetLegalEntityBanksAuditView(ObjectContext context, int legalEntityFK) 
        {
            IBanksRepository banksRepository = new BanksRepository(context); 
            ILegalEntitiesRepository LegalEntitiesRepository = new LegalEntitiesRepository(context);
            ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(context);

            // get all legalEntity bank records
            List<LegalEntityBankView> legalEntitiesBanks = LegalEntityBankView.GetLegalEntityBankView(legalEntitiesBanksRepository.GetAll(), banksRepository.GetValid(), LegalEntitiesRepository.GetValidLegalEntities())
                                                            .Where(c => c.LegalEntityFK == legalEntityFK)
                                                            .ToList();

            List<List<LegalEntityBankAuditView>> LegalEntityBanksListList = new List<List<LegalEntityBankAuditView>>();

            foreach(LegalEntityBankView legalEntityBank in legalEntitiesBanks)
            {
                LegalEntityBanksListList.Add(LegalEntityBankAuditView.GetLegalEntityBankAuditView(context, legalEntityBank.LegalEntityBankPK));
            }

            return LegalEntityBanksListList;
        }


        public static List<LegalEntityBankAuditView> GetLegalEntityBankAuditView(ObjectContext context, int relatedEntityPK) 
        {
            IAuditingDetailsRepository auditingDetailsRepository = new AuditingDetailsRepository(context); 
            IAuditingMasterRepository auditingMasterRepository = new AuditingMasterRepository(context);

            var sessionTokens = (from am in auditingMasterRepository.GetAll().Where(c => c.TableName == "LegalEntityBanks")
                                    where am.RelatedEntityPK == relatedEntityPK
                                    select new { 
                                    AuditingMasterPK = am.AuditingMasterPK, 
                                    RelatedEntityPK = am.RelatedEntityPK, 
                                    SessionToken = am.SessionToken 
                                }).ToList();

            List<LegalEntityBankAuditView> legalEntityBankAuditViewList = new List<LegalEntityBankAuditView>();

            foreach (var item in sessionTokens) 
            {
                var record = auditingDetailsRepository.GetAuditingDetailByAuditingMasterPK(item.AuditingMasterPK).ToList();

                LegalEntityBankAuditView legalEntityBankAuditView = new LegalEntityBankAuditView();

                legalEntityBankAuditView.BankFK = record.checkInteger("BankFK");
                legalEntityBankAuditView.Iban = record.checkString("Iban");
                legalEntityBankAuditView.ChangeDate = record.checkDate("ChangeDate");
                legalEntityBankAuditView.Deleted = record.checkBoolean("Deleted");

                legalEntityBankAuditViewList.Add(legalEntityBankAuditView);
            }
                        
            IBanksRepository banksRepository = new BanksRepository(context); 
            IQueryable<Bank> banksTable = banksRepository.GetValid();

            List<LegalEntityBankAuditView> legalEntityBanks =  
                                                ( from t in legalEntityBankAuditViewList
                                                from t1 in banksTable.Where(tbl => tbl.BankPK == t.BankFK).DefaultIfEmpty()
                                                where t.ChangeDate != null
                                                select new LegalEntityBankAuditView
                                                {
                                                    LegalEntityBankPK   = t.LegalEntityBankPK,
                                                    BankFK              = t.BankFK,
                                                    BankIban            = t.Iban,
                                                    BankName            = t1 != null && t1.Name != null ? t1.Name : null,
                                                    ChangeDate        = t.ChangeDate != null ? t.ChangeDate : null,
                                                    Deleted             = t.Deleted != null ? t.Deleted : null,
                                                }).OrderBy(c => c.ChangeDate).ToList();

            return legalEntityBanks;
        }

    }
}
