namespace FlareWorks.Models.Users
{
    /// <summary> Information about the last item that was added by this user </summary>
    public class UserInfo_Recents_LastAdded
    {
        /// <summary> Name of the form used when the last item was added by this user </summary>
        public string FormType { get; set; }

        /// <summary> Catalog type for the last item added by this user </summary>
        public string CatalogType { get; set; }

        /// <summary> Institution for the last item added by this user </summary>
        public string Institution { get; set; }

        /// <summary> Bibliographic level for the last item added by this user </summary>
        public string BiblioLevel { get; set; }

        /// <summary> Material type for the last item added by this user </summary>
        public string MaterialType { get; set; }
    }
}
