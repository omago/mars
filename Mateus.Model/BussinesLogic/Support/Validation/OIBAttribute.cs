using System.ComponentModel.DataAnnotations;

namespace Mateus.Model.BussinesLogic.Support.Validation
{
    public class OIBAttribute : RegularExpressionAttribute
    { 
        const string pattern = @"^(\d{11})$";

        public OIBAttribute()
            : base(pattern)
        {
            this.ErrorMessage = "Not a valid OIB";
        }
    }
}