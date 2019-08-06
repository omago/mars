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

namespace Mateus.Model.BussinesLogic.Views.AnnexContractModel
{
    public class AnnexContractView
    {
        public int AnnexContractPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ugovor je obavezan.")]
        public int? ContractFK { get; set; }

        [Required(ErrorMessage = "Broj ugovora je obavezan.")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Datum ugovora je obavezan.")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Valjanost ugovora je obavezna.")]
        public int? ContractValidityFK { get; set; }

        [Required(ErrorMessage = "Primjena ugovora je obavezna.")]
        public DateTime? AnnexContractBegin { get; set; }

        [RequiredIf("ContractValidityFK == 1", ErrorMessage="Istek ugovora je obavezan.")]
        public DateTime? AnnexContractEnd { get; set; }

        [RequiredIf("Terminated == true", ErrorMessage="Datum raskida ugovora je obavezan.")]
        public DateTime? TerminationDate { get; set; }

        [RequiredIf("Terminated == true", ErrorMessage="Razlog raskida ugovora je obavezan.")]
        public string TerminationDescription { get; set; }

        public bool? Terminated { get; set; }

        public bool? Deleted { get; set; }

        public int? CurrencyFK { get; set; }
        public decimal? MIO { get; set; }
        public decimal? TSI { get; set; }
        public decimal? GZR { get; set; }
        public decimal? OPL { get; set; }
        public decimal? AUP { get; set; }

        public IEnumerable<SelectListItem> ContractValidities { get; set; }
        public IEnumerable<SelectListItem> Contracts { get; set; }
        public IEnumerable<SelectListItem> Currencies { get; set; }

        public string ContractName { get; set; }

        public void ConvertFrom(AnnexContract annexContract, AnnexContractView annexContractView) 
        {
            annexContractView.AnnexContractPK          = annexContract.AnnexContractPK;
            annexContractView.ContractFK               = annexContract.ContractFK;
            annexContractView.Name                     = annexContract.Name;
            annexContractView.Number                   = annexContract.Number;
            annexContractView.Date                     = annexContract.Date;
            annexContractView.ContractValidityFK       = annexContract.ContractValidityFK;
            annexContractView.AnnexContractBegin       = annexContract.AnnexContractBegin;
            annexContractView.AnnexContractEnd         = annexContract.AnnexContractEnd;
            annexContractView.TerminationDate          = annexContract.TerminationDate;
            annexContractView.TerminationDescription   = annexContract.TerminationDescription;
            annexContractView.Terminated               = annexContract.Terminated;
            annexContractView.CurrencyFK               = annexContract.CurrencyFK;
            annexContractView.MIO                      = annexContract.MIO;
            annexContractView.TSI                      = annexContract.TSI;
            annexContractView.GZR                      = annexContract.GZR;
            annexContractView.OPL                      = annexContract.OPL;
            annexContractView.AUP                      = annexContract.AUP;
            annexContractView.Deleted                  = annexContract.Deleted;
        }

        public void ConvertTo(AnnexContractView annexContractView, AnnexContract annexContract) 
        {
            annexContract.AnnexContractPK           = annexContractView.AnnexContractPK;
            annexContract.ContractFK                = annexContractView.ContractFK;
            annexContract.Name                      = annexContractView.Name;
            annexContract.Number                    = annexContractView.Number;
            annexContract.Date                      = annexContractView.Date;
            annexContract.ContractValidityFK        = annexContractView.ContractValidityFK;
            annexContract.AnnexContractBegin        = annexContractView.AnnexContractBegin;
            annexContract.AnnexContractEnd          = annexContractView.AnnexContractEnd;
            annexContract.TerminationDate           = annexContractView.TerminationDate;
            annexContract.TerminationDescription    = annexContractView.TerminationDescription;
            annexContract.Terminated                = annexContractView.Terminated;
            annexContract.CurrencyFK                = annexContractView.CurrencyFK;
            annexContract.MIO                       = annexContractView.MIO;
            annexContract.TSI                       = annexContractView.TSI;
            annexContract.GZR                       = annexContractView.GZR;
            annexContract.OPL                       = annexContractView.OPL;
            annexContract.AUP                       = annexContractView.AUP;
            annexContract.Deleted                   = annexContractView.Deleted;
        }

        public void BindDDLs(AnnexContractView annexContractView, ObjectContext db) 
        {
            //contract validities ddl
            IContractValiditiesRepository contractValiditiesRepository = new ContractValiditiesRepository(db);
            annexContractView.ContractValidities = new SelectList(contractValiditiesRepository.GetValid().ToList(), "ContractValidityPK", "Name");

            //contracts ddl
            IContractsRepository contractsRepository = new ContractsRepository(db);
            annexContractView.Contracts = new SelectList(contractsRepository.GetValid().ToList(), "ContractPK", "Name");

            //currencies ddl
            ICurrenciesRepository currenciesRepository = new CurrenciesRepository(db);
            annexContractView.Currencies = new SelectList(currenciesRepository.GetValid().OrderBy(c => c.Name).Select(c => new { value = c.CurrencyPK, text = c.Name + " (" + c.Sign + ")" }), "value", "text");
        }

        public static IQueryable<AnnexContractView> GetAnnexContractView(IQueryable<AnnexContract> annexContractTable, IQueryable<Contract> contractTable) 
        {
            IQueryable<AnnexContractView> annexContractViewList = (from t1 in annexContractTable
                                       join t2 in contractTable on t1.ContractFK equals t2.ContractPK

                                       select new AnnexContractView
                                       {
                                            AnnexContractPK     = t1.AnnexContractPK,
                                            Name                = t1.Name,
                                            Date                = t1.Date,
                                            AnnexContractBegin  = t1.AnnexContractBegin,
                                            AnnexContractEnd    = t1.AnnexContractEnd,
                                            Number              = t1.Number,
                                            ContractName        = t2.Name,
                                            ContractFK          = t1.ContractFK
                                       }).AsQueryable<AnnexContractView>();

            return annexContractViewList;
        }
    }
}
