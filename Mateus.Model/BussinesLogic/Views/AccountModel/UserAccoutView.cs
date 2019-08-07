using System;
using System.Collections.Generic;
using System.Linq;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.BussinesLogic.Support.Validation;

namespace Mateus.Model.BussinesLogic.Views.Account
{
    public class UserAccountView
    {
        #region Properties

        public int? UserPK { get; set; }

        [Required(ErrorMessage = "Korisničko ime je obavezno."), Display(Name = "Korisničko ime")]
        public string Username { get; set; }

        public bool ChangePassword { get; set; }

        [RequiredIf("ChangePassword", ErrorMessage = "Šifra je obavezna."), Display(Name = "Šifra"), DataType(DataType.Password)]
        public string Password { get; set; }
        public string Salt { get; set; }

        [RequiredIf("ChangePassword", ErrorMessage = "Potvrda šifre je obavezna."), Display(Name = "Potvrda šifre"), DataType(DataType.Password), System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Unesene šifre nisu jednake.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "E-mail adresa je obavezna."), Display(Name = "E-Mail"), Email(ErrorMessage = "Email adresa nije ispravna.")]
        public string Email { get; set; }

        [Display(Name = "Datum registracije")]
        public DateTime? RegistrationDate { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Korisničko pravo pristupa je obavezno.")]
        public int? RolePK { get; set; }

        public IEnumerable<SelectListItem> Roles;

        #endregion

        #region Initializers

        public UserAccountView()
        {

        }

        #endregion

        #region Helpers

        public void ConvertFrom(User user, UserAccountView userAccountView)
        {
            userAccountView.UserPK = user.UserPK;
            userAccountView.Username = user.Username;
            userAccountView.Password = user.Password;
            userAccountView.Salt = user.Salt;
            userAccountView.Deleted = user.Deleted;
            userAccountView.Active = user.Active;
            userAccountView.RegistrationDate = user.RegistrationDate;
            userAccountView.FirstName = user.FirstName;
            userAccountView.LastName = user.LastName;
            userAccountView.Email = user.Email;
        }

        public void ConvertTo(UserAccountView userAccountView, User user)
        {
            user.Username = userAccountView.Username;
            user.Password = userAccountView.Password;
            user.Salt = userAccountView.Salt;
            user.Deleted = userAccountView.Deleted;
            user.Active = userAccountView.Active;
            user.RegistrationDate = userAccountView.RegistrationDate;
            user.FirstName = userAccountView.FirstName;
            user.LastName = userAccountView.LastName;
            user.Email = userAccountView.Email;
        }

        public static IQueryable<UserAccountView> GetUserAccountView(IQueryable<User> validUsers)
        {
            IQueryable<UserAccountView> userAccountView = (from u in validUsers
                                                           select new UserAccountView
                                                             {
                                                                 UserPK = u.UserPK,
                                                                 Username = u.Username,
                                                                 Password = u.Password,
                                                                 Email = u.Email,
                                                                 RegistrationDate = u.RegistrationDate,
                                                                 Active = u.Active,
                                                                 Deleted = u.Deleted,
                                                                 FirstName = u.FirstName,
                                                                 LastName = u.LastName
                                                             }).AsQueryable<UserAccountView>();

            return userAccountView;
        }
        #endregion
    }
}
