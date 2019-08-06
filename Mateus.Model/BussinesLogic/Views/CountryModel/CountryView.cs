using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.CountryModel
{
    public class CountryView
    {
        public int CountryPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Naziv [en] je obavezan."), StringLength(256, ErrorMessage = "Naziv [en] ne smije biti duži od 256 znakova.")]
        public string NameEn { get; set; }

        [Required(ErrorMessage = "Državljanstvo je obavezno."), StringLength(256, ErrorMessage = "Državljanstvo ne smije biti duže od 256 znakova.")]
        public string Citizenship { get; set; }

        [Required(ErrorMessage = "ISO 3166-1 alpha 2 šifra je obavezna."), StringLength(2, ErrorMessage = "ISO 3166-1 alpha 2 šifra ne smije biti duža od 2 znaka.")]
        public string Alpha2Code { get; set; }

        [Required(ErrorMessage = "ISO 3166-1 alpha 3 šifra je obavezna."), StringLength(3, ErrorMessage = "ISO 3166-1 alpha 3 šifra ne smije biti duža od 3 znaka.")]
        public string Alpha3Code { get; set; }

        [Required(ErrorMessage = "ISO 3166-1 numerička šifra je obavezna."), StringLength(3, ErrorMessage = "ISO 3166-1 numerička šifra ne smije biti duža od 3 znaka.")]
        public string NumericCode { get; set; }

        public bool? Risk { get; set; }

        public bool? Deleted { get; set; }

        public void ConvertFrom(Country country, CountryView countryView) 
        {
            countryView.CountryPK   = country.CountryPK;
            countryView.Name        = country.Name;
            countryView.NameEn      = country.NameEn;
            countryView.Citizenship = country.Citizenship;
            countryView.Alpha2Code  = country.Alpha2Code;
            countryView.Alpha3Code  = country.Alpha3Code;
            countryView.NumericCode = country.NumericCode;
            countryView.Risk        = country.Risk;
            countryView.Deleted     = country.Deleted;
        }

        public void ConvertTo(CountryView countryView, Country country) 
        {
            country.Name        = countryView.Name;
            country.NameEn      = countryView.NameEn;
            country.Citizenship = countryView.Citizenship;
            country.Alpha2Code  = countryView.Alpha2Code;
            country.Alpha3Code  = countryView.Alpha3Code;
            country.NumericCode = countryView.NumericCode;
            country.Risk        = countryView.Risk;
        }
    }
}
