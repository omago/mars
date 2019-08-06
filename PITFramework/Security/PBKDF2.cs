using System;
using System.Security.Cryptography;

namespace PITFramework.Security
{
    public class PBKDF2
    {       
        #region Declarations

        // default values
        public string Password { get; set; }
        public int SaltLength { get; set; }
        public int KeyLength { get; set; }
        public int NumberOfIterations { get; set; }
        public byte[] SaltBytes { get; set; }

        #endregion

        #region Initializers

        /// <summary>
        /// PBKDF2 constructor
        /// </summary>
        /// <param name="password">Password</param>
        public PBKDF2(string password) 
        {
            Password = password;
            SaltLength = 8;
            KeyLength = 32;
            NumberOfIterations = 10000;
        }

        /// <summary>
        /// PBKDF2 constructor
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="saltBytes">Salt bytes</param>
        public PBKDF2(string password, byte[] saltBytes)
        {
            Password = password;
            SaltBytes = saltBytes;
            SaltLength = saltBytes.Length;
            KeyLength = 32;
            NumberOfIterations = 10000;
        }

        /// <summary>
        /// PBKDF2 constructor
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="saltBytes">Salt bytes</param>
        /// <param name="saltLength">Salt length</param>
        /// <param name="keyLength">Key length</param>
        /// <param name="numberOfIterations">Number of iterations</param>
        public PBKDF2(string password, byte[] saltBytes, int saltLength, int keyLength, int numberOfIterations)
        {
            Password = password;
            SaltBytes = saltBytes;
            SaltLength = saltLength;
            KeyLength = keyLength;
            NumberOfIterations = numberOfIterations;
        }

        #endregion

        #region PBKDF2 password generation

        /// <summary>
        /// Computes salt 
        /// </summary>
        /// <param name="saltLength">Salt length (default value = 8 bytes)</param>
        /// <returns>Computed salt</returns>
        public byte[] ComputeSalt(int saltLength = 8) 
        {
            byte[] saltBytes = new byte[saltLength];

            // Create instance of pseudo radnom function generator (PRF)
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();   

            // Fill saltBytes with PRF 
            rngCsp.GetBytes(saltBytes);

            return saltBytes;
        }

        /// <summary>
        /// Computes PBKDF2 based password and salt
        /// </summary>
        /// <param name="password">User password</param>
        /// <param name="salt">Generated salt</param>
        /// <returns>Computed PBKDF2 based password</returns>
        public byte[] ComputePBKDF2()
        {
            // Compute salt
            SaltBytes = ComputeSalt();

            return computePBKDF2(Password, SaltBytes, NumberOfIterations);
        }

        /// <summary>
        /// Computes PBKDF2 based password
        /// </summary>
        /// <param name="saltBytes">Salt bytes</param>
        /// <returns>Computed PBKDF2 based password</returns>
        public byte[] ComputePBKDF2(byte[] saltBytes)
        {
            if (saltBytes == null || saltBytes.Length == 0)
            {
                throw new Exception("Salt empty!");
            }
            
            if (saltBytes.Length < 8)
            {
                throw new Exception("Salt is not at least eight bytes!");
            }

            SaltBytes = saltBytes;

            return computePBKDF2(Password, SaltBytes, NumberOfIterations);
        }

        /// <summary>
        /// Computes PBKDF2 based password 
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="saltBytes">Salt bytes</param>
        /// <param name="numberOfIterations">Number of iterations</param>
        /// <returns>Computed PBKDF2 based password</returns>     
        private byte[] computePBKDF2(string password, byte[] saltBytes, int numberOfIterations) 
        {
            // Calculate PBKDF2 based password
            Rfc2898DeriveBytes PBKDF2 = new Rfc2898DeriveBytes(Password, SaltBytes, NumberOfIterations);

            return PBKDF2.GetBytes(KeyLength);
        }

        /// <summary>
        /// Verifies provided password against stored password
        /// </summary>
        /// <param name="dbPassword">Stored password</param>
        /// <returns>True if passwords matches, else False</returns>
        public bool VerifyPassword(string storedPassword)
        {
            string computedPassword = Convert.ToBase64String(ComputePBKDF2(SaltBytes));

            if (computedPassword == storedPassword) return true;
            else return false;
        }

        #endregion
    }
}