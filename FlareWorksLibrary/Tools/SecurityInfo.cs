using System;
using System.Security.Cryptography;
using System.Text;

namespace FlareWorks.Library.Tools
{
    public static class SecurityInfo
    {
        /// <summary> Encrypt a string, given the string.  </summary>
        /// <param name="Source"> String to encrypt </param>
        /// <returns> The encrypted string </returns>
        public static string SHA1_EncryptString(string Source)
        {
            byte[] bytIn = Encoding.ASCII.GetBytes(Source);

            // set the private key
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] bytOut = sha.ComputeHash(bytIn);

            // convert into Base64 so that the result can be used in xml
            return Convert.ToBase64String(bytOut, 0, bytOut.Length);
        }
    }
}
