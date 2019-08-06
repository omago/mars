using System.ComponentModel.DataAnnotations;

namespace Mateus.Model.BussinesLogic.Support.Validation
{
    public class WebAddressAttribute : RegularExpressionAttribute
    { 
        const string pattern = @"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";

        public WebAddressAttribute()
            : base(pattern)
        {
            this.ErrorMessage = "Not a valid web address";
        }
    }
}