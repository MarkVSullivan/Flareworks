namespace FlareWorks.Models.Users
{
    /// <summary> Information about the last search that was run by this user </summary>
    public class UserInfo_Recents_LastSearch
    {
        /// <summary> Last location utilized for a search, by this user </summary>
        public string Location { get; set; }

        /// <summary> Last form type utilized for a search, by this user </summary>
        public string FormType { get; set; }

        /// <summary> Last institution utilized for a search, by this user </summary>
        public string Institution { get; set; }

        /// <summary> Last criteria utilized for a search, by this user </summary>
        public string ByCriteria { get; set; }
    }
}
