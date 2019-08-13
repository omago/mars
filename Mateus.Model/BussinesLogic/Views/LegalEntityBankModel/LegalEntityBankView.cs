using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;
using PITFramework.Support;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityBankModel
{
    public class LegalEntityBankView
    {
        public int LegalEntityBankPK { get; set; }

        [Required(ErrorMessage = "Tvrtka je obavezna.")]
        public int? LegalEntityFK { get; set; }

        [Required(ErrorMessage = "Banka je obavezna.")]
        public int? BankFK { get; set; }

        [Required(ErrorMessage = "IBAN je obavezan."), StringLength(32, ErrorMessage = "IBAN ne smije biti duži od 32 znakova.")]
        public string Iban { get; set; }

        [Required(ErrorMessage = "Datum rješenja je obavezan.")]
        public DateTime? ChangeDate { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> Banks { get; set; }

        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIban { get; set; }
        public string LegalEntityName { get; set; }

        public void ConvertFrom(LegalEntityBank LegalEntityBank, LegalEntityBankView LegalEntityBankView, ObjectContext db) 
        {
            LegalEntityBankView.LegalEntityBankPK   = LegalEntityBank.LegalEntityBankPK;
            LegalEntityBankView.LegalEntityFK       = LegalEntityBank.LegalEntityFK;
            LegalEntityBankView.BankFK              = LegalEntityBank.BankFK;
            LegalEntityBankView.Iban                = LegalEntityBank.Iban;
            LegalEntityBankView.Deleted             = LegalEntityBank.Deleted;

            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)LegalEntityBankView.LegalEntityFK);
            LegalEntityBankView.LegalEntityName = legalEntity.Name + " (" + legalEntity.OIB + ")";
        }

        public void ConvertTo(LegalEntityBankView LegalEntityBankView, LegalEntityBank LegalEntityBank) 
        {
            LegalEntityBank.LegalEntityBankPK   = LegalEntityBankView.LegalEntityBankPK;
            LegalEntityBank.LegalEntityFK       = LegalEntityBankView.LegalEntityFK;
            LegalEntityBank.BankFK              = LegalEntityBankView.BankFK;
            LegalEntityBank.Iban                = LegalEntityBankView.Iban;
            LegalEntityBank.ChangeDate          = LegalEntityBankView.ChangeDate;
            LegalEntityBank.Deleted             = LegalEntityBankView.Deleted;
        }

        public void BindDLLs(LegalEntityBankView LegalEntityBankView, ObjectContext db) 
        {
            //bank ddl
            IBanksRepository banksRepository = new BanksRepository (db);
            LegalEntityBankView.Banks = new SelectList(banksRepository.GetValid().OrderBy("Name ASC").ToList(), "BankPK", "Name");
        }

        public static IQueryable<LegalEntityBankView> GetLegalEntityBankView(IQueryable<LegalEntityBank> legalEntityBankTable, IQueryable<Bank> bankTable, IQueryable<LegalEntity> legalEntitiesTable) 
        {
            IQueryable<LegalEntityBankView> legalEntityBankViewList = (from t1 in legalEntityBankTable
                                       join t2 in bankTable on t1.BankFK equals t2.BankPK
                                       join t3 in legalEntitiesTable on t1.LegalEntityFK equals t3.LegalEntityPK

                                       select new LegalEntityBankView
                                       {
                                            LegalEntityBankPK   = t1.LegalEntityBankPK,
                                            LegalEntityFK       = t1.LegalEntityFK,
                                            BankFK              = t1.BankFK,
                                            Iban                = t1.Iban,
                                            LegalEntityName     = t3.Name,
                                            BankName            = t2.Name,
                                            BankAccountNumber   = t2.AccountNumber,
                                            BankIban            = t1.Iban,
                                       }).AsQueryable<LegalEntityBankView>();

            return legalEntityBankViewList;
        }
    }
}
