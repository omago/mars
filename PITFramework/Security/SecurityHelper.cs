using System;
using System.Web;
using System.Web.Security;
using PITFramework.Support;

namespace PITFramework.Security
{
    public static class SecurityHelper
    {
        public static string[] GetUserRolesFromCookie()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            string[] userRoles = null;
            FormsAuthenticationTicket authTicket = null;
            if (authCookie != null)
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                string userRolesString = authTicket.UserData.Split('|')[0]; // roles[0] | userID[1]

                userRoles = StringHelper.SplitString(userRolesString);
            }

            return userRoles;
        }

        public static int GetUserPKFromCookie()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket authTicket = null;
            string userPKString = "";
            if (authCookie != null)
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                userPKString = authTicket.UserData.Split('|')[1]; // roles[0] | userID[1]
            }

            if (!String.IsNullOrWhiteSpace(userPKString)) return Convert.ToInt32(userPKString);
            else return 0; // default value
        }
    }
}
