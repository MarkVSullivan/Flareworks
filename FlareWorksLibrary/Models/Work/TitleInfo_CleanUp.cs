using System;

namespace FlareWorks.Models.Work
{
    /// <summary> Single clean-up task associated with a title, including history of being added
    /// to the title and (possibly) subsequently cleared </summary>
    public class TitleInfo_CleanUp
    {
        /// <summary> Type of the clean-up task </summary>
        public string CleanUpType { get; set; }

        /// <summary> Date this task was selected to be completed against this title </summary>
        public DateTime DateAdded { get; set; }

        /// <summary> User that added this clean-up task to this title </summary>
        public string AddedByUser { get; set; }

        /// <summary> Date this task was cleared, or NULL if not cleared </summary>
        public DateTime? DateCleared { get; set; }

        /// <summary> User that cleared this task </summary>
        public string ClearedByUser { get; set; }

        /// <summary> Any description, included in the OTHER category of record clean-up </summary>
        public string OtherDescription { get; set; }

        /// <summary> Primary key for this record clean-up </summary>
        public int PrimaryKey { get; set; }

        /// <summary> Constructor for a new instance of the TitleInfo_CleanUp class </summary>
        public TitleInfo_CleanUp()
        {
            // Empty
        }

        /// <summary> Constructor for a new instance of the TitleInfo_CleanUp class </summary>
        /// <param name="CleanUpType"> Type of the clean-up task </param>
        public TitleInfo_CleanUp(string CleanUpType)
        {
            this.CleanUpType = CleanUpType;
        }

        /// <summary> Constructor for a new instance of the TitleInfo_CleanUp class </summary>
        /// <param name="PrimaryKey"> Primary key for this record clean-up </param>
        /// <param name="CleanUpType"> Type of the clean-up task </param>
        public TitleInfo_CleanUp(int PrimaryKey, string CleanUpType)
        {
            this.PrimaryKey = PrimaryKey;
            this.CleanUpType = CleanUpType;
        }
    }
}
