using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Mateus.Model.BussinesLogic.Support.Validation
{
    public class LessThenSystemDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt = (DateTime)value;
            if (dt < DateTime.Today)
            {
                return new ValidationResult(this.ErrorMessage);
            }

            return ValidationResult.Success;
        }

    }
}
