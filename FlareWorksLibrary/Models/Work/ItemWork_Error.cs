using System;

namespace FlareWorks.Models.Work
{
    /// <summary> Single error associated with item/work, including history of being added
    /// to the title and (possibly) subsequently cleared </summary>
    public class ItemWork_Error
    {
        /// <summary> Type of the error, associated with a single item/work </summary>
        public string ErrorType { get; set; }

        /// <summary> Date this error was added to the item/work </summary>
        public DateTime DateAdded { get; set; }

        /// <summary> User that added this error to the item/work </summary>
        public string AddedByUser { get; set; }

        /// <summary> Date this error was cleared, or NULL if not cleared </summary>
        public DateTime? DateCleared { get; set; }

        /// <summary> User that cleared this error </summary>
        public string ClearedByUser { get; set; }
    }
}
