using System.ComponentModel.DataAnnotations;

namespace Mateus.Model.BussinesLogic.Support.Validation
{
    public class JMBGAttribute : RegularExpressionAttribute
    { 
        const string pattern = @"^(\d{11})$";

        public JMBGAttribute()
            : base(pattern)
        {
            this.ErrorMessage = "Not a valid JMBG";
        }
    }
}