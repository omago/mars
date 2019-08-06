using System.ComponentModel.DataAnnotations;

namespace Mateus.Model.BussinesLogic.Support.Validation
{
    public class GiroAccountAttribute : RegularExpressionAttribute
    {
        const string pattern = @"^(\d{7})+(\-)+(\d{10})$";

        public GiroAccountAttribute()
            : base(pattern)
        {
            this.ErrorMessage = "Not a valid \"Giro\" account";
        }
    }
}