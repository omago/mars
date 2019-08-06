using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Mateus.Model.BussinesLogic.Support.Validation
{
    public class DateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        { 
            bool isValid = false;
            DateTime temp = DateTime.MinValue;

            if (value != null && !String.IsNullOrWhiteSpace((string)value))
            {
                if (DateTime.TryParse((string)value, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out temp)) isValid = true;
            }

            if (isValid) return ValidationResult.Success;
            else return new ValidationResult("Not valid date time.");
        }

    }
}
