using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.BussinesLogic.Support.Validation;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;

namespace Mateus.Model.BussinesLogic.Views.ContractModel
{
    public class ContractView
    {
        public int ContractPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Tvrtka je obavezna.")]
        public int? LegalEntityFK { get; set; }

        [Required(ErrorMessage = "Valjanost ugovora je obavezna.")]
        public int? ContractValidityFK { get; set; }

        [Required(ErrorMessage = "Broj ugovora je obavezan.")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Datum ugovora je obavezan.")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Primjena ugovora je obavezna.")]
        public DateTime? ContractBegin { get; set; }

        [RequiredIf("ContractValidityFK == 1", ErrorMessage="Datum isteka ugovora je obavezan.")]
        public DateTime? ContractEnd { get; set; }

        [RequiredIf("Terminated == true", ErrorMessage="Datum raskida ugovora je obavezan.")]
        public DateTime? TerminationDate { get; set; }

        [RequiredIf("Terminated == true", ErrorMessage="Razlog raskida ugovora je obavezan.")]
        public string TerminationDescription { get; set; }

        public bool? Terminated { get; set; }

        public int? CurrencyFK { get; set; }
        public decimal? MIO { get; set; }
        public decimal? TSI { get; set; }
        public decimal? GZR { get; set; }
        public decimal? OPL { get; set; }
        public decimal? AUP { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> ContractValidities { get; set; }
        public IEnumerable<SelectListItem> Currencies { get; set; }

        public string LegalEntityName { get; set; }

        public int? AnnexContractsCount { get; set; }

        public void ConvertFrom(Contract contract, ContractView contractView, ObjectContext db) 
        {
            contractView.ContractPK             = contract.ContractPK;
            contractView.LegalEntityFK          = contract.LegalEntityFK;
            contractView.Name                   = contract.Name;
            contractView.Number                 = contract.Number;
            contractView.Date                   = contract.Date;
            contractView.ContractValidityFK     = contract.ContractValidityFK;
            contractView.ContractBegin          = contract.ContractBegin;
            contractView.ContractEnd            = contract.ContractEnd;
            contractView.TerminationDate        = contract.TerminationDate;
            contractView.TerminationDescription = contract.TerminationDescription;
            contractView.Terminated             = contract.Terminated;
            contractView.CurrencyFK             = contract.CurrencyFK;
            contractView.MIO                    = contract.MIO;
            contractView.TSI                    = contract.TSI;
            contractView.GZR                    = contract.GZR;
            contractView.OPL                    = contract.OPL;
            contractView.AUP                    = contract.AUP;
            contractView.Deleted                = contract.Deleted;

            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)contractView.LegalEntityFK);
            contractView.LegalEntityName = legalEntity.Name + " (" + legalEntity.OIB + ")";
        }

        public void ConvertTo(ContractView contractView, Contract contract) 
        {
            contract.ContractPK             = contractView.ContractPK;
            contract.LegalEntityFK          = contractView.LegalEntityFK;
            contract.Name                   = contractView.Name;
            contract.Number                 = contractView.Number;
            contract.Date                   = contractView.Date;
            contract.ContractValidityFK     = contractView.ContractValidityFK;
            contract.ContractBegin          = contractView.ContractBegin;
            contract.ContractEnd            = contractView.ContractEnd;
            contract.TerminationDate        = contractView.TerminationDate;
            contract.TerminationDescription = contractView.TerminationDescription;
            contract.Terminated             = contractView.Terminated;
            contract.CurrencyFK             = contractView.CurrencyFK;
            contract.MIO                    = contractView.MIO;
            contract.TSI                    = contractView.TSI;
            contract.GZR                    = contractView.GZR;
            contract.OPL                    = contractView.OPL;
            contract.AUP                    = contractView.AUP;
            contract.Deleted                = contractView.Deleted;
        }

        public void BindDDLs(ContractView contractView, ObjectContext db) 
        {
            //contract validities ddl
            IContractValiditiesRepository contractValiditiesRepository = new ContractValiditiesRepository(db);
            contractView.ContractValidities = new SelectList(contractValiditiesRepository.GetValid().ToList(), "ContractValidityPK", "Name");

            //currencies ddl
            ICurrenciesRepository currenciesRepository = new CurrenciesRepository(db);
            contractView.Currencies = new SelectList(currenciesRepository.GetValid().OrderBy(c => c.Name).Select(c => new { value = c.CurrencyPK, text = c.Name + " (" + c.Sign + ")" }), "value", "text");
        }

        public static IQueryable<ContractView> GetContractsView(IQueryable<Contract> contractTable, IQueryable<AnnexContract> annexContractTable, IQueryable<LegalEntity> legalEntityTable) 
        {
            IQueryable<ContractView> contractViewList = (from t1 in contractTable
                                       join t2 in legalEntityTable on t1.LegalEntityFK equals t2.LegalEntityPK

                                       select new ContractView
                                       {
                                            ContractPK          = t1.ContractPK,
                                            Name                = t1.Name,
                                            Date                = t1.Date,
                                            ContractBegin       = t1.ContractBegin,
                                            ContractEnd         = t1.ContractEnd,
                                            Number              = t1.Number,
                                            LegalEntityName     = t2.Name,
                                            LegalEntityFK       = t1.LegalEntityFK,
                                            AnnexContractsCount = annexContractTable.Where(b => b.ContractFK == t1.ContractPK).Count(),
                                       }).AsQueryable<ContractView>();

            return contractViewList;
        }
    }
}