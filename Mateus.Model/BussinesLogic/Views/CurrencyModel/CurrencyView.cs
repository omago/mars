using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.CurrencyModel
{
    public class CurrencyView
    {
        public int CurrencyPK { get; set; }

        [Required(ErrorMessage = "Naziv valute je obavezan."), StringLength(256, ErrorMessage = "Naziv valute ne može biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Simbol valute je obavezan."), StringLength(256, ErrorMessage = "Simbol valute ne može biti duži od 10 znakova.")]
        public string Sign { get; set; }

        public bool? Deleted { get; set; }

        public void ConvertFrom(Currency currency, CurrencyView currencyView) 
        {
            currencyView.CurrencyPK = currency.CurrencyPK;
            currencyView.Name       = currency.Name;
            currencyView.Sign       = currency.Sign;
            currencyView.Deleted    = currency.Deleted;
        }

        public void ConvertTo(CurrencyView currencyView, Currency currency) 
        {
            currency.CurrencyPK = currencyView.CurrencyPK;
            currency.Name       = currencyView.Name;
            currency.Sign       = currencyView.Sign;
            currency.Deleted    = currencyView.Deleted;
        }
    }
}
