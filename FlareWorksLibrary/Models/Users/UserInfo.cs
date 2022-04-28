using System;
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
    }
}
