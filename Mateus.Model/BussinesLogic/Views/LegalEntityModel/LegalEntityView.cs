using Mateus.Model.BussinesLogic.Support.Validation;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using PITFramework.Support;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityModel
{
    public class LegalEntityView
    {
        public int LegalEntityPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Kratki naziv je obavezan."), StringLength(3, ErrorMessage = "Kratki naziv ne smije biti duži od 3 znaka.")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "OIB je obavezan."), OIB(ErrorMessage = "OIB nije ispravnan.")]
        public string OIB { get; set; }

        [StringLength(8, ErrorMessage = "MB ne smije biti duži od 8 znakova.")]
        public string MB { get; set; }

        [Required(ErrorMessage = "MBS je obavezan."), StringLength(9, ErrorMessage = "MBS ne smije biti duži od 8 znakova.")]
        public string MBS { get; set; }

        [RequiredIf("Company == true", ErrorMessage = "Oblik je obavezan.")]
        public int? FormFK { get; set; }


        public bool? Company { get; set; } = true;
        public bool? Active { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Forms { get; set; }
        public IEnumerable<SelectListItem> RegionalOffices { get; set; }

        public int? BranchesCount { get; set; }
        public int? ContractsCount { get; set; }
        public int? BanksCount { get; set; }
        public int? LegalRepresentativesCount { get; set; }
        public int? LegalEntityOwnersCount { get; set; }
        public int? AssessmentsCount { get; set; }

        public string FormName { get; set; }

        public string BankName { get; set; }
        public int? BankFK { get; set; }

        public void ConvertFrom(LegalEntity legalEntity, LegalEntityView legalEntityView)
        {
            legalEntityView.LegalEntityPK = legalEntity.LegalEntityPK;
            legalEntityView.Name = legalEntity.Name;
            legalEntityView.ShortName = legalEntity.ShortName;
            legalEntityView.OIB = legalEntity.OIB;
            legalEntityView.MB = legalEntity.MB;
            legalEntityView.MBS = legalEntity.MBS;

            legalEntityView.FormFK = legalEntity.FormFK;

            legalEntityView.Company = legalEntity.Company;
            legalEntityView.Active = legalEntity.Active;

            legalEntityView.Deleted = legalEntity.Deleted;
        }

        public void ConvertTo(LegalEntityView legalEntityView, LegalEntity legalEntity)
        {
            legalEntity.LegalEntityPK = legalEntityView.LegalEntityPK;
            legalEntity.Name = legalEntityView.Name;
            legalEntity.ShortName = legalEntityView.ShortName;
            legalEntity.OIB = legalEntityView.OIB;
            legalEntity.MB = legalEntityView.MB;
            legalEntity.MBS = legalEntityView.MBS;

            legalEntity.FormFK = legalEntityView.FormFK;

            legalEntity.Company = legalEntityView.Company;
            legalEntity.Active = legalEntityView.Active;

            legalEntity.Deleted = legalEntityView.Deleted;
        }

        public void BindDDLs(LegalEntityView legalEntityView, ObjectContext db)
        {
            //forms ddl
            IFormsRepository formsRepository = new FormsRepository(db);
            legalEntityView.Forms = new SelectList(formsRepository.GetValid().OrderBy("Name ASC").ToList(), "FormPK", "Name");

            //activities ddl
            IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);
            var activities = activitiesRepository.GetValid().OrderBy("Name ASC").ToList();

            //regional offices ddl
            IRegionalOfficesRepository regionalOfficesRepository = new RegionalOfficesRepository(db);
            legalEntityView.RegionalOffices = new SelectList(regionalOfficesRepository.GetValid().OrderBy("Name ASC").ToList(), "RegionalOfficePK", "Name");
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
                                                                   Active = t1.Active,
                                                                   LegalEntityPK = t1.LegalEntityPK,
                                                                   Name = t1.Name + " (" + t1.OIB + ")",
                                                                   Company = t1.Company,
                                                                   BranchesCount = branchTable.Where(b => b.LegalEntityFK == t1.LegalEntityPK).Count(),
                                                                   ContractsCount = contractTable.Where(c => c.LegalEntityFK == t1.LegalEntityPK).Count(),
                                                                   BanksCount = (from mn in legalEntityBankTable
                                                                                 from c1 in legalEntityTable.Where(c2 => c2.LegalEntityPK == mn.LegalEntityFK).DefaultIfEmpty()
                                                                                 from b1 in bankTable.Where(b2 => b2.BankPK == mn.BankFK).DefaultIfEmpty()
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

            if (legalEntitiesFiltered.Count() > 0)
            {
                if (!string.IsNullOrWhiteSpace(Name)) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByName(Name); }
                if (!string.IsNullOrWhiteSpace(OIB)) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByOIB(OIB); }
                if (!string.IsNullOrWhiteSpace(MB)) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByMB(MB); }
                if (!string.IsNullOrWhiteSpace(MBS)) { legalEntitiesFiltered = legalEntitiesFiltered.GetLegalEntitiesByMBS(MBS); }

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
                                                                   FormName = t1.Form.Name != null ? t1.Form.Name : ""
                                                               }).Distinct().AsQueryable<LegalEntityView>();

            return legalEntityViewList;
        }

        public static LegalEntityView GetLegalEntityReport(ObjectContext db, int legalEntityPK)
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK(legalEntityPK);

            LegalEntityView legalEntityView = new LegalEntityView
            {
                LegalEntityPK = legalEntity.LegalEntityPK,
                Name = legalEntity.Name,
                ShortName = legalEntity.ShortName,
                OIB = legalEntity.OIB,
                MB = legalEntity.MB,
                MBS = legalEntity.MBS,
                FormName = legalEntity.FormFK != null ? legalEntity.Form.Name : null,

                Active = legalEntity.Active,
                Deleted = legalEntity.Deleted
            };

            return legalEntityView;
        }
    }
}
