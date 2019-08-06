using System.ComponentModel.DataAnnotations;

namespace Mateus.Model.Account
{
    public class LogOn
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

}
