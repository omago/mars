using System.ComponentModel.DataAnnotations;

namespace Mateus.Model.BussinesLogic.Support.Validation
{
    public class EmailAttribute : RegularExpressionAttribute
    { 
        const string pattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

        public EmailAttribute()
            : base(pattern)
        {
            this.ErrorMessage = "Not a valid e-mail address";
        }
    }
}