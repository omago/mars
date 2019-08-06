using System;
using System.Text;
using System.Security.Cryptography;

namespace PITFramework.Security
{
    public class CryptographyHelper
    {
        #region Hash functions

        /// <summary>
        /// Computes SHA1 Hash
        /// </summary>
        /// <param name="plainText">Plain text to hash</param>
        /// <returns>Base64 encoded hash</returns>
        public static string ComputeSHA1HashBase64(string plainText) 
        {
            using (SHA1 sha1 = new SHA1CryptoServiceProvider()) 
            {
                byte[] hash = sha1.ComputeHash(UTF8Encoding.UTF8.GetBytes(plainText));

                return Convert.ToBase64String(hash);
            }   
        }

        #endregion
    }
}
