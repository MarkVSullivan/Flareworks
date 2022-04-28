using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FlareWorks.Models.ControlledValues;

namespace FlareWorks.Models.Users
{
    /// <summary> Information about a single user of the Flareworks system </summary>
    public class UserInfo
    {
        /// <summary> Primary key for this user from the database </summary>
        public int PrimaryKey { get; set; }

        /// <summary> Username for this user, which also uniquely identifies this user </summary>
        public string UserName { get; set; }

        /// <summary> Display name used throughout the system </summary>
        public string DisplayName { get; set; }

        /// <summary> Full name used throughout the system </summary>
        public string FullName { get; set; }

        /// <summary> Primary work location for this user ( i.e., ALF, ILF, etc.. ) </summary>
        public LocationInfo Location { get; set; }

        /// <summary> Email address for this user </summary>
        public string Email { get; set; }

        /// <summary> Date this user was registered within the system </summary>
        public DateTime DateAdded { get; set; }

        /// <summary> Flag indicates this user is currently pending approval </summary>
        public bool PendingApproval { get; set; }

        /// <summary> Flag indicates this user is completely disabled, and can no longer sign on </summary>
        public bool Disabled { get; set; }

        /// <summary> Flag indicates if this is a temporary password that needs to be reset </summary>
        public bool TemporaryPassword { get; set; }

        /// <summary> Specific permissions for this user, dictating what actions they
        /// can perform, once approved </summary>
        public UserInfo_Permissions Permissions { get; private set; }

        /// <summary> Recently used values by a single user, allowing data input to rememeber
        /// the last entered values or the last search criteria used </summary>
        public UserInfo_Recents Recents { get; private set; }

        /// <summary> Constructor for a new instance of the <see cref="UserInfo"/> class </summary>
        public UserInfo()
        {
            Permissions = new UserInfo_Permissions();
            Recents = new UserInfo_Recents();
        }

        /// <summary> Returns the security hash based on IP for this user </summary>
        /// <param name="IP">IP Address for this user request</param>
        /// <returns>Security hash for comparison purposes or for encoding in the cookie</returns>
        /// <remarks>This is used to add another level of security on cookies coming in from a user request </remarks>
        public string Security_Hash(string IP)
        {
            return DES_EncryptString(DisplayName + "flareh" + PrimaryKey + "worksh" + UserName, IP.Replace(".", "").PadRight(8, '%').Substring(0, 8), Email.Length > 8 ? Email.Substring(0, 8) : Email.PadLeft(8, 'd'));
        }

        /// <summary> Encrypt a string, given the string.  </summary>
        /// <param name="Source"> String to encrypt </param>
        /// <param name="Key"> Key for the encryption </param>
        /// <param name="Iv"> Initialization Vector for the encryption </param>
        /// <returns> The encrypted string </returns>
        public static string DES_EncryptString(string Source, string Key, string Iv)
        {
            byte[] bytIn = Encoding.ASCII.GetBytes(Source);
            // create a MemoryStream so that the process can be done without I/O files
            MemoryStream ms = new MemoryStream();

            // set the private key
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(Key),
                IV = Encoding.ASCII.GetBytes(Iv)
            };

            // create an Encryptor from the Provider Service instance
            ICryptoTransform encrypto = desProvider.CreateEncryptor();

            // create Crypto Stream that transforms a stream using the encryption
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

            // write out encrypted content into MemoryStream
            cs.Write(bytIn, 0, bytIn.Length);
            cs.Close();

            // Write out from the Memory stream to an array of bytes
            byte[] bytOut = ms.ToArray();
            ms.Close();

            // convert into Base64 so that the result can be used in xml
            return Convert.ToBase64String(bytOut, 0, bytOut.Length);
        }
    }
}
