using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.BussinesLogic.Support.Validation;
using Mateus.Model.BussinesLogic.Support.DDL;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;
using System.Data.Objects.SqlClient;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityOwnerModel
{
    public class LegalEntityOwnerView
    {
        public int LegalEntityOwnerPK { get; set; }

        [Required(ErrorMessage = "Tvrtka je obavezna.")]
        public int? LegalEntityFK { get; set; }

        public int? OwnerFK { get; set; }

        [RequiredIf("LegalEntityFK != null", ErrorMessage = "Vršni vlasnik je obavezan.")]
        public string OwnerStringFK { get; set; }

        public string OwnerType { get; set; }

        [Required(ErrorMessage = "Iznos poslovnog udjela je obavezan.")]
        public decimal? BusinessShareAmount { get; set; }

        [Required(ErrorMessage = "Iznos uplaćenih sredstava poslovnog udjela je obavezan.")]
        public decimal? PaidBussinesShareAmount { get; set; }

        [Required(ErrorMessage = "Nominalni iznos poslovnog udjela je obavezan.")]
        public decimal? NominalBussinesShareAmount { get; set; }

        [Required(ErrorMessage = "Dodatni čimbenik je obavezan.")]
        public int? AdditionalFactorFK { get; set; }

        [Required(ErrorMessage = "Ispunjeni čimbenik je obavezan.")]
        public int? FulfilledFactorFK { get; set; }

        [Required(ErrorMessage = "Teret na poslovnom udjelu je obavezan.")]
        public int? BussinesShareBurdenFK { get; set; }

        [Required(ErrorMessage = "Vrsta promjene je obavezna.")]
        public int? ChangeTypeFK { get; set; }

        [Required(ErrorMessage = "Broj glasova je obavezan.")]
        public int? NumberOfVotes { get; set; }

        [Required(ErrorMessage = "Datum unosa je obavezan.")]
        public DateTime? EntryDate { get; set; }

        [Required(ErrorMessage = "Datum rješenja je obavezan.")]
        public DateTime? ChangeDate { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Owners { get; set; }
        public IEnumerable<SelectListItem> AdditionalFactors { get; set; }
        public IEnumerable<SelectListItem> FulfilledFactors { get; set; }
        public IEnumerable<SelectListItem> BussinesShareBurdens { get; set; }
        public IEnumerable<SelectListItem> ChangeTypes { get; set; }

        public string LegalEntityName { get; set; }
        public string OwnerName { get; set; }
        public string OwnerFirstname { get; set; }
        public string OwnerLastname { get; set; }
        public string OwnerLegalEntityName { get; set; }
        public string OwnerChildName { get; set; }

        public void ConvertFrom(LegalEntityOwner legalEntityOwner, LegalEntityOwnerView legalEntityOwnerView, ObjectContext db) 
        {
            legalEntityOwnerView.LegalEntityOwnerPK         = legalEntityOwner.LegalEntityOwnerPK;

            legalEntityOwnerView.LegalEntityFK              = legalEntityOwner.LegalEntityFK;

            legalEntityOwnerView.OwnerStringFK              = legalEntityOwner.OwnerType.Trim() + "|" + legalEntityOwner.OwnerFK.ToString();

            legalEntityOwnerView.AdditionalFactorFK         = legalEntityOwner.AdditionalFactorFK;
            legalEntityOwnerView.FulfilledFactorFK          = legalEntityOwner.FulfilledFactorFK;
            legalEntityOwnerView.BussinesShareBurdenFK      = legalEntityOwner.BussinesShareBurdenFK;
            legalEntityOwnerView.ChangeTypeFK               = legalEntityOwner.ChangeTypeFK;

            legalEntityOwnerView.BusinessShareAmount        = legalEntityOwner.BusinessShareAmount;
            legalEntityOwnerView.NominalBussinesShareAmount = legalEntityOwner.NominalBussinesShareAmount;
            legalEntityOwnerView.PaidBussinesShareAmount    = legalEntityOwner.PaidBussinesShareAmount;

            legalEntityOwnerView.NumberOfVotes              = legalEntityOwner.NumberOfVotes;
            
            legalEntityOwnerView.EntryDate                  = legalEntityOwner.EntryDate;

            legalEntityOwnerView.Deleted                    = legalEntityOwner.Deleted;

            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityOwnerView.LegalEntityFK);
            legalEntityOwnerView.LegalEntityName = legalEntity.Name + " (" + legalEntity.OIB + ")";
        }

        public void ConvertTo(LegalEntityOwnerView legalEntityOwnerView, LegalEntityOwner legalEntityOwner) 
        {
            legalEntityOwner.LegalEntityOwnerPK         = legalEntityOwnerView.LegalEntityOwnerPK;

            legalEntityOwner.LegalEntityFK              = legalEntityOwnerView.LegalEntityFK;
            
            string[] tmpOwnerTypes                      = legalEntityOwnerView.OwnerStringFK.Split('|');
            legalEntityOwner.OwnerType                  = tmpOwnerTypes[0].Trim();
            legalEntityOwner.OwnerFK                    = Convert.ToInt32(tmpOwnerTypes[1]);

            legalEntityOwner.AdditionalFactorFK         = legalEntityOwnerView.AdditionalFactorFK;
            legalEntityOwner.FulfilledFactorFK          = legalEntityOwnerView.FulfilledFactorFK;
            legalEntityOwner.BussinesShareBurdenFK      = legalEntityOwnerView.BussinesShareBurdenFK;
            legalEntityOwner.ChangeTypeFK               = legalEntityOwnerView.ChangeTypeFK;

            legalEntityOwner.BusinessShareAmount        = legalEntityOwnerView.BusinessShareAmount;
            legalEntityOwner.NominalBussinesShareAmount = legalEntityOwnerView.NominalBussinesShareAmount;
            legalEntityOwner.PaidBussinesShareAmount    = legalEntityOwnerView.PaidBussinesShareAmount;

            legalEntityOwner.NumberOfVotes              = legalEntityOwnerView.NumberOfVotes;
            legalEntityOwner.EntryDate                  = legalEntityOwnerView.EntryDate;

            legalEntityOwner.ChangeDate               = legalEntityOwnerView.ChangeDate;

            legalEntityOwner.Deleted                    = legalEntityOwnerView.Deleted;
        }

        public void BindDLLs(LegalEntityOwnerView legalEntityOwnerView, ObjectContext db) 
        {
            //Owners ddl
            IPhysicalEntitiesRepository physicalEntitesRepository = new PhysicalEntitiesRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);

            List<DDLHelper> owners = LegalEntityOwnerView.GetOwnersSelect(physicalEntitesRepository.GetValid(), legalEntitiesRepository.GetValidOwners()).ToList();
            legalEntityOwnerView.Owners = new SelectList(owners, "Value", "Text");
                
            //AdditionalFactors ddl
            IAdditionalFactorsRepository additionalFactorsRepository = new AdditionalFactorsRepository(db);
            legalEntityOwnerView.AdditionalFactors = new SelectList(additionalFactorsRepository.GetValid().ToList(), "AdditionalFactorPK", "Name");

            //FulfilledFactors ddl
            IFulfilledFactorsRepository fulfilledFactorsRepository = new FulfilledFactorsRepository(db);
            legalEntityOwnerView.FulfilledFactors = new SelectList(fulfilledFactorsRepository.GetValid().ToList(), "FulfilledFactorPK", "Name");

            //BussinesShareBurdens ddl
            IBussinesShareBurdensRepository bussinesShareBurdensRepository = new BussinesShareBurdensRepository(db);
            legalEntityOwnerView.BussinesShareBurdens = new SelectList(bussinesShareBurdensRepository.GetValid().ToList(), "BussinesShareBurdenPK", "Name");

            //ChangeTypes ddl
            IChangeTypesRepository changeTypesRepository = new ChangeTypesRepository(db);
            legalEntityOwnerView.ChangeTypes = new SelectList(changeTypesRepository.GetValid().ToList(), "ChangeTypePK", "Name");
        }

        public static void Fill(List<LegalEntityOwner> list, List<LegalEntityOwner> all, LegalEntityOwner co) 
        {
            var allParents = all.Where( co1 => co1.LegalEntityFK == co.OwnerFK);

            if (allParents != null && allParents.Count() > 0)
            {
                list.Add(co);

                foreach (var parent in allParents) 
                {
                    Fill(list, all, parent);
                }
            }
            else 
            {
                list.Add(co);
                return;
            }
        }

        public static IQueryable<LegalEntityOwnerView> GetLegalEntityOwnerView(IQueryable<LegalEntityOwner> legalEntityOwnerTable, IQueryable<PhysicalEntity> physicalEntityTable, IQueryable<LegalEntity> legalEntityTable)
        {
            IQueryable<LegalEntityOwnerView> peViewList = (from co in legalEntityOwnerTable
                                                       from c in legalEntityTable.Where(c1 => c1.LegalEntityPK == co.LegalEntityFK).DefaultIfEmpty()
                                                       from pe in physicalEntityTable.Where(pe1 => pe1.PhysicalEntityPK == co.OwnerFK).DefaultIfEmpty()
                                                       where co.OwnerType.Contains("pe")
                                                       select new LegalEntityOwnerView
                                                       {
                                                           LegalEntityOwnerPK = co.LegalEntityOwnerPK,
                                                           OwnerFK = co.OwnerFK,
                                                           LegalEntityFK = co.LegalEntityFK,
                                                           LegalEntityName = c.Name,
                                                           OwnerName = pe.Lastname + " " +  pe.Firstname
                                                       }).AsQueryable<LegalEntityOwnerView>();

            IQueryable<LegalEntityOwnerView> leViewList = (from co in legalEntityOwnerTable
                                                       from c in legalEntityTable.Where(c1 => c1.LegalEntityPK == co.LegalEntityFK).DefaultIfEmpty()
                                                       from le in legalEntityTable.Where(le1 => le1.LegalEntityPK == co.OwnerFK).DefaultIfEmpty()
                                                       where co.OwnerType.Contains("le")
                                                       select new LegalEntityOwnerView
                                                       {
                                                           LegalEntityOwnerPK = co.LegalEntityOwnerPK,
                                                           OwnerFK = co.OwnerFK,
                                                           LegalEntityFK = co.LegalEntityFK,
                                                           LegalEntityName = c.Name,
                                                           OwnerName = le.Name
                                                       }).AsQueryable<LegalEntityOwnerView>();


            var legalEntityRelations = peViewList.Union(leViewList);

            return legalEntityRelations;
        }

        public static IQueryable<DDLHelper> GetOwnersSelect(IQueryable<PhysicalEntity> physicalEntityTable, IQueryable<LegalEntity> legalEntityTable) 
        { 
            var physicalEntities = physicalEntityTable.Select(pe => new DDLHelper
                                                                    {
                                                                        Text = pe.Lastname + " " + pe.Firstname,
                                                                        Value = "pe|" + SqlFunctions.StringConvert((double)pe.PhysicalEntityPK).Trim()
                                                                    });

            var legalEntities = legalEntityTable.Select(le => new DDLHelper
                                                            {
                                                                Text = le.Name,
                                                                Value = "le|" + SqlFunctions.StringConvert((double)le.LegalEntityPK).Trim()
                                                            });

            var owners = physicalEntities.Union(legalEntities).OrderBy(o => o.Text);

            return owners;
        }

        public static IQueryable<LegalEntityOwner> GetLegalEntityOwnersForLegalEntity(int legalEntityFK, List<LegalEntityOwner> coList, IQueryable<LegalEntityOwner> legalEntityOwnersTable) 
        {
            List<LegalEntityOwner> filteredCoList = new List<LegalEntityOwner>();

            foreach (var co in coList)
            {
                LegalEntityOwnerView.Fill(filteredCoList, legalEntityOwnersTable.ToList(), co);
            }

            legalEntityOwnersTable = filteredCoList.AsQueryable();

            return legalEntityOwnersTable;
        }

        public static IQueryable<LegalEntity> GetRelatedLegalEntities(IQueryable<LegalEntityOwner> legalEntityOwnersTable, IQueryable<LegalEntity> legalEntityTable, IQueryable<Country> countryTable, IQueryable<Activity> activityTable)
        {
            IQueryable<LegalEntity> leViewList = (from t in legalEntityOwnersTable
                                                  from t1 in legalEntityTable.Where(le => le.LegalEntityPK == t.OwnerFK).DefaultIfEmpty()
                                                  where t.OwnerType.Contains("le")
                                                  select t1).AsQueryable<LegalEntity>();

            return leViewList;

        }
    }
}
