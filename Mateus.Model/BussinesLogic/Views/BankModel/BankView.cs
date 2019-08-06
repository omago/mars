using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.BankModel
{
    public class BankView
    {
        public int? BankPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Broj računa je obavezan."), StringLength(256, ErrorMessage = "Broj računa ne smije biti duži od 256 znakova.")]
        [Display(Name="Account number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "SWIFT je obavezan."), StringLength(256, ErrorMessage = "SWIFT ne smije biti duži od 256 znakova.")]
        public string Swift { get; set; }

        [Required(ErrorMessage = "OIB je obavezan."), StringLength(256, ErrorMessage = "OIB ne smije biti duži od 256 znakova.")]
        public string Oib { get; set; }

        [Required(ErrorMessage = "IBAN je obavezan."), StringLength(256, ErrorMessage = "IBAN ne smije biti duži od 256 znakova.")]
        public string Iban { get; set; }
        
        public bool? Deleted { get; set; }

        public void ConvertFrom(Bank bank, BankView bankView) 
        {
            bankView.BankPK = bank.BankPK;
            bankView.Name = bank.Name;
            bankView.AccountNumber = bank.AccountNumber;
            bankView.Swift = bank.Swift;
            bankView.Oib = bank.Oib;
            bankView.Iban = bank.Iban;
            bankView.Deleted = bank.Deleted;
        }

        public void ConvertTo(BankView bankView, Bank bank) 
        {
            bank.Name = bankView.Name;
            bank.AccountNumber = bankView.AccountNumber;
            bank.Swift = bankView.Swift;
            bank.Oib = bankView.Oib;
            bank.Iban = bankView.Iban;
        }
    }
}
