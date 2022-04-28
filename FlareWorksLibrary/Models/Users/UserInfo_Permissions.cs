namespace FlareWorks.Models.Users
{
    /// <summary> Specific permissions for this user, dictating what actions they
    /// can perform, once approved </summary>
    public class UserInfo_Permissions
    {
        /// <summary> User can perform quality control actions </summary>
        public bool CanQC { get; set; }

        /// <summary> User can run reports </summary>
        public bool CanRunReports { get; set; }

        /// <summary> User is a system administrator, allowing the controlled lists to be edited and users approved </summary>
        public bool IsSystemAdmin { get; set; }

        /// <summary> User can add items to the pull lists </summary>
        public bool CanAddToPullLists { get; set; }

        /// <summary> User is a pull list adminstrator </summary>
        public bool IsPullListAdmin { get; set; }

        /// <summary> User can perform advanced searches </summary>
        public bool CanAdvancedSearch { get; set; }

        /// <summary> User should see the PCC Category during title input/edit </summary>
        public bool CatalogingSpecialist { get; set; }

        /// <summary> Check to see if this user can process items </summary>
        public bool CanProcessItems
        {
            get
            {
                return true;
            }
        }

        /// <summary> Constructor for a new instance of the <see cref="UserInfo_Permissions"/> class </summary>
        public UserInfo_Permissions()
        {
            CanQC = false;
            CanRunReports = false;
            IsSystemAdmin = false;
            CanAddToPullLists = false;
            IsPullListAdmin = false;
            CanAdvancedSearch = false;
        }

    }
}
