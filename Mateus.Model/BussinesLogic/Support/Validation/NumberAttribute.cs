using System.ComponentModel.DataAnnotations;

namespace Mateus.Model.BussinesLogic.Support.Validation
{
    public class NumberAttribute : RegularExpressionAttribute
    { 
        const string pattern = @"\d*";

        public NumberAttribute()
            : base(pattern)
        {
            this.ErrorMessage = "Not a valid number";
        }
    }
}
