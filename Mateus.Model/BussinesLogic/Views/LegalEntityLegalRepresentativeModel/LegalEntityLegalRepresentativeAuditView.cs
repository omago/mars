using System;
using System.Collections.Generic;
using System.Linq;
using Mateus.Model.EFModel;
using System.Data.Objects;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.BussinesLogic.Views.LegalEntityLegalRepresentativeModel;
using Mateus.Model.BussinesLogic.Support.ExtensionMethods;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityLegalRepresentativeAuditModel
{
    public class LegalEntityLegalRepresentativeAuditView
    {
        public int LegalEntityLegalRepresentativePK { get; set; }

        public int? LegalEntityFK { get; set; }

        public int? WayOfRepresentationFK { get; set; }

        public int? LegalRepresentativeFK { get; set; }

        public DateTime? ChangeDate { get; set; }

        public bool? Deleted { get; set; }

        public string LegalEntityName { get; set; }
        public string LegalRepresentativeName { get; set; }
        public string WayOfRepresentationName { get; set; }

        public static List<List<LegalEntityLegalRepresentativeAuditView>> GetLegalEntityLegalRepresentativesAuditView(ObjectContext context, int legalEntityFK) 
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(context);
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(context);
            ILegalEntityLegalRepresentativesRepository legalEntityLegalRepresentativesRepository = new LegalEntityLegalRepresentativesRepository(context);

            // get all legalEntity branches
            List<LegalEntityLegalRepresentativeView> legalEntityLegalRepresentatives = LegalEntityLegalRepresentativeView.GetLegalEntityLegalRepresentativeView(legalEntityLegalRepresentativesRepository.GetAll(), legalEntitiesRepository.GetValid(), physicalEntitiesRepository.GetValid())
                                                            .Where(c => c.LegalEntityFK == legalEntityFK)
                                                            .ToList();

            List<List<LegalEntityLegalRepresentativeAuditView>> legalEntityLegalRepresentativesListList = new List<List<LegalEntityLegalRepresentativeAuditView>>();

            foreach(LegalEntityLegalRepresentativeView legalEntityLegalRepresentative in legalEntityLegalRepresentatives)
            {
                legalEntityLegalRepresentativesListList.Add(LegalEntityLegalRepresentativeAuditView.GetLegalEntityLegalRepresentativeAuditView(context, legalEntityLegalRepresentative.LegalEntityLegalRepresentativePK));
            }

            return legalEntityLegalRepresentativesListList;
        }


        public static List<LegalEntityLegalRepresentativeAuditView> GetLegalEntityLegalRepresentativeAuditView(ObjectContext context, int relatedEntityPK) 
        {
            IAuditingDetailsRepository auditingDetailsRepository = new AuditingDetailsRepository(context); 
            IAuditingMasterRepository auditingMasterRepository = new AuditingMasterRepository(context);

            var sessionTokens = (from am in auditingMasterRepository.GetAll().Where(c => c.TableName == "LegalEntityLegalRepresentatives")
                                    where am.RelatedEntityPK == relatedEntityPK
                                    select new { 
                                    AuditingMasterPK = am.AuditingMasterPK, 
                                    RelatedEntityPK = am.RelatedEntityPK, 
                                    SessionToken = am.SessionToken 
                                }).ToList();

            List<LegalEntityLegalRepresentativeAuditView> legalEntityLegalRepresentativeAuditViewList = new List<LegalEntityLegalRepresentativeAuditView>();

            foreach (var item in sessionTokens) 
            {
                var record = auditingDetailsRepository.GetAuditingDetailByAuditingMasterPK(item.AuditingMasterPK).ToList();

                LegalEntityLegalRepresentativeAuditView legalEntityLegalRepresentativeAuditView = new LegalEntityLegalRepresentativeAuditView();

                legalEntityLegalRepresentativeAuditView.LegalEntityFK = record.checkInteger("LegalEntityFK");
                legalEntityLegalRepresentativeAuditView.LegalRepresentativeFK = record.checkInteger("LegalRepresentativeFK");
                legalEntityLegalRepresentativeAuditView.WayOfRepresentationFK = record.checkInteger("WayOfRepresentationFK");

                legalEntityLegalRepresentativeAuditView.ChangeDate = record.checkDate("ChangeDate");
                legalEntityLegalRepresentativeAuditView.Deleted = record.checkBoolean("Deleted");

                legalEntityLegalRepresentativeAuditViewList.Add(legalEntityLegalRepresentativeAuditView);
            }

            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(context);
            IQueryable<PhysicalEntity> physicalEntityTable = physicalEntitiesRepository.GetValid();

            IWaysOfRepresentationRepository WaysOfRepresentationRepository = new WaysOfRepresentationRepository(context);
            IQueryable<WayOfRepresentation> wayOfRepresentationTable = WaysOfRepresentationRepository.GetValid();

            List<LegalEntityLegalRepresentativeAuditView> legalEntityLegalRepresentative =  
                                            ( from t in legalEntityLegalRepresentativeAuditViewList
                                            from t1 in physicalEntityTable.Where(tbl => tbl.PhysicalEntityPK == t.LegalRepresentativeFK).DefaultIfEmpty()
                                            from t2 in wayOfRepresentationTable.Where(tbl => tbl.WayOfRepresentationPK == t.WayOfRepresentationFK).DefaultIfEmpty()
                                            where t.ChangeDate != null
                                            select new LegalEntityLegalRepresentativeAuditView
                                            {
                                                LegalEntityLegalRepresentativePK    = t.LegalEntityLegalRepresentativePK,

                                                LegalRepresentativeName             = t1 != null && t1.Firstname != null && t1.Lastname != null  ? t1.Firstname + " " + t1.Lastname : null,
                                                WayOfRepresentationName             = t2 != null && t2.Name != null ? t2.Name : null,

                                                ChangeDate                        = t.ChangeDate != null ? t.ChangeDate : null,
                                                Deleted                             = t.Deleted != null ? t.Deleted : null,
                                            }).OrderBy(c => c.ChangeDate).ToList();

            return legalEntityLegalRepresentative;
        }
    }
}
