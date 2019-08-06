using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;
using Mateus.Model.BussinesLogic.Views.WorkDoneModel;

namespace Mateus.Model.EFModel.Repository
{
    public static class RepositoryExtensionMethods
    {
        #region LegalEntity

        public static IQueryable<LegalEntity> GetLegalEntitiesByName(this IQueryable<LegalEntity> legalEntities, string name)
        {
            return legalEntities.Where(c => c.Name.Contains(name));
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByOIB(this IQueryable<LegalEntity> legalEntities, string OIB)
        {
            return legalEntities.Where(c => c.OIB.Contains(OIB));
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByMB(this IQueryable<LegalEntity> legalEntities, string MB)
        {
            return legalEntities.Where(c => c.MB.Contains(MB));
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByMBS(this IQueryable<LegalEntity> legalEntities, string MBS)
        {
            return legalEntities.Where(c => c.MBS.Contains(MBS));
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByTax(this IQueryable<LegalEntity> legalEntities, int? taxPK)
        {
            return legalEntities.Where(c => c.TaxFK == taxPK);
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByForm(this IQueryable<LegalEntity> legalEntities, int? formPK)
        {
            return legalEntities.Where(c => c.FormFK == formPK);
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByActivity(this IQueryable<LegalEntity> legalEntities, int? activityPK)
        {
            return legalEntities.Where(c => c.ActivityFK == activityPK);
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesBySubstation(this IQueryable<LegalEntity> legalEntities, int? substationPK)
        {
            return legalEntities.Where(c => c.SubstationFK == substationPK);
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByCommercialCourt(this IQueryable<LegalEntity> legalEntities, int? commercialCourtPK)
        {
            return legalEntities.Where(c => c.CommercialCourtFK == commercialCourtPK);
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByTouristOffice(this IQueryable<LegalEntity> legalEntities, bool? TouristOffice)
        {
            return legalEntities.Where(c => c.TouristOffice == true);
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByMonumentAnnuity(this IQueryable<LegalEntity> legalEntities, bool? MonumentAnnuity)
        {
            return legalEntities.Where(c => c.MonumentAnnuity == true);
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByEmployeesRange(this IQueryable<LegalEntity> legalEntities, int? numberOfEmployeesFrom, int? numberOfEmployeesTo)
        {
            if (numberOfEmployeesFrom != null)
            {
                legalEntities = legalEntities.Where(c => c.NumberOfEmployees >= numberOfEmployeesFrom);
            }

            if (numberOfEmployeesTo != null)
            {
                legalEntities = legalEntities.Where(c => c.NumberOfEmployees <= numberOfEmployeesTo);
            }

            return legalEntities;
        }


        public static IQueryable<LegalEntity> GetLegalEntitiesByFundamentalCapitalRange(this IQueryable<LegalEntity> legalEntities, int? fundamentalCapitalFrom, int? fundamentalCapitalTo)
        {
            if (fundamentalCapitalFrom != null)
            {
                legalEntities = legalEntities.Where(c => c.FundamentalCapital >= fundamentalCapitalFrom);
            }

            if (fundamentalCapitalTo != null)
            {
                legalEntities = legalEntities.Where(c => c.FundamentalCapital <= fundamentalCapitalTo);
            }

            return legalEntities;
        }


        public static IQueryable<LegalEntity> GetLegalEntitiesByBank(this IQueryable<LegalEntity> legalEntities, ObjectContext db, int? bankPK)
        {
            IBanksRepository banksRepository = new BanksRepository(db);
            ILegalEntityBanksRepository legalEntityBanksRepository = new LegalEntityBanksRepository(db);

            var banks = banksRepository.GetValid();
            var legalEntityBanks = legalEntityBanksRepository.GetValid();

            var legalEntitiesByBank = (from mn in legalEntityBanks
                                   from b in banks.Where(b => b.BankPK == mn.BankFK).DefaultIfEmpty()
                                   from c in legalEntities.Where(c => c.LegalEntityPK == mn.LegalEntityFK).DefaultIfEmpty()
                                   where b.BankPK == bankPK
                                   select c);

            return legalEntitiesByBank;
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesReport(ObjectContext db, string ordering, string Name, string OIB, string MB, string MBS, int? BankPK, int? TaxPK, int? FormPK, int? ActivityPK, int? numberOfEmployeesFrom, int? numberOfEmployeesTo)
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IBanksRepository banksRepository = new BanksRepository(db);
            ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);

            var banks = banksRepository.GetValid();
            var legalEntitiesBanks = legalEntitiesBanksRepository.GetValid();
            var allLegalEntities = legalEntitiesRepository.GetValid();

            // Applying filters
            IQueryable<LegalEntity> legalEntitiesFiltered = allLegalEntities; // default list

            // Banks
            if (BankPK != null)
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByBank(db, BankPK);
            }

            // Tax payers
            if (TaxPK != null)
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByTax(TaxPK);
            }

            // Forms
            if (FormPK != null)
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByForm(FormPK);
            }

            // ACtivities
            if (ActivityPK != null)
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByActivity(ActivityPK);
            }

            // Name filter
            if (!String.IsNullOrWhiteSpace(Name))
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByName(Name);
            }

            // OIB filter
            if (!String.IsNullOrWhiteSpace(OIB))
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByOIB(OIB);
            }

            // MB filter
            if (!String.IsNullOrWhiteSpace(MB))
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByMB(MB);
            }

            // MBS filter
            if (!String.IsNullOrWhiteSpace(MBS))
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByMBS(MBS);
            }

            // numberOfEmployees
            if (numberOfEmployeesFrom != null || numberOfEmployeesTo != null)
            {
                legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByEmployeesRange(numberOfEmployeesFrom, numberOfEmployeesTo);
            }

            return legalEntitiesFiltered;

        }

        #endregion

        #region WorkDone

        public static IQueryable<WorkDone> GetWorkDonesByToDoList(this IQueryable<WorkDone> workDones, int? toDoListFK)
        {
            return workDones.Where(wd => wd.ToDoListFK == toDoListFK);
        }

        public static IQueryable<WorkDone> GetWorkDoneByLegalEntity(this IQueryable<WorkDone> workDones, int? legalEntityFK)
        {
            return workDones.Where(wd => wd.LegalEntityFK == legalEntityFK);
        }

        public static IQueryable<WorkDone> GetWorkDoneByWorkType(this IQueryable<WorkDone> workDones, int? workTypeFK)
        {
            return workDones.Where(wd => wd.WorkTypeFK == workTypeFK);
        }

        public static IQueryable<WorkDone> GetWorkDonesByWorkSubtype(this IQueryable<WorkDone> workDones, int? workSubtypeFK)
        {
            return workDones.Where(wd => wd.WorkSubtypeFK == workSubtypeFK);
        }

        public static IQueryable<WorkDone> GetWorkDonesByServiceType(this IQueryable<WorkDone> workDones, int? serviceTypeFK)
        {
            return workDones.Where(wd => wd.ServiceTypeFK == serviceTypeFK);
        }

        public static IQueryable<WorkDone> GetWorkDonesByUser(this IQueryable<WorkDone> workDones, int? userFK)
        {
            return workDones.Where(wd => wd.UserFK == userFK);
        }

        public static IQueryable<WorkDone> GetWorkDonesByDateFrom(this IQueryable<WorkDone> workDones, DateTime? dateFrom)
        {
            return workDones.Where(wd => wd.Date >= dateFrom);
        }

        public static IQueryable<WorkDone> GetWorkDonesByDateTo(this IQueryable<WorkDone> workDones, DateTime? dateTo)
        {
            return workDones.Where(wd => wd.Date <= dateTo);
        }

        public static IQueryable<WorkDone> GetWorkDonesByTimeSpentRange(this IQueryable<WorkDone> workDones, int? timeSpentFrom, int? timeSpentTo)
        {
            if (timeSpentFrom != null)
            {
                workDones = workDones.Where(wd => wd.TimeSpent >= timeSpentFrom);
            }

            if (timeSpentTo != null)
            {
                workDones = workDones.Where(wd => wd.TimeSpent <= timeSpentTo);
            }

            return workDones;
        }

        public static IQueryable<WorkDone> GetWorkDonesByNumberOfAttachmentsRange(this IQueryable<WorkDone> workDones, IQueryable<WorkDoneView> numberOfAttachmentsList, int? numberOfAttachmentsFrom, int? numberOfAttachmentsTo)
        {
            List<int> workDonesPKs = new List<int>();
            if (numberOfAttachmentsFrom != null)
            {
                numberOfAttachmentsList = numberOfAttachmentsList.Where(atListCount => atListCount.WorkDoneAttachmentsCount >= numberOfAttachmentsFrom);
            }

            if (numberOfAttachmentsTo != null)
            {
                numberOfAttachmentsList = numberOfAttachmentsList.Where(atListCount => atListCount.WorkDoneAttachmentsCount <= numberOfAttachmentsTo);
            }

            workDonesPKs = numberOfAttachmentsList.Select( al => al.WorkDonePK ).ToList();

            workDones = workDones.Where(wd => workDonesPKs.Contains(wd.WorkDonePK));

            return workDones;
        }

        public static IQueryable<WorkDone> GetWorkDonesByDescription(this IQueryable<WorkDone> workDones, string description)
        {
            return workDones.Where(c => c.Description.Contains(description));
        }

        #endregion
    }
}