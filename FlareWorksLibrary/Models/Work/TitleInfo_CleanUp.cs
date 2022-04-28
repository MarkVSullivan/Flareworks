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
    }
}
