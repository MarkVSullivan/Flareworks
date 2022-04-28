using System;

namespace FlareWorks.Library.Models.QC
{
    /// <summary> Single error associated with item/work, including history of being added
    /// to the title and (possibly) subsequently cleared </summary>
    public class WorkSet_Error
    {
        /// <summary> Type of the error, associated with a single item/work </summary>
        public string ErrorType { get; set; }

        /// <summary> Description of this type of error </summary>
        public string ErrorDescripton { get; set; }

        /// <summary> Primary key of this error type from the database </summary>
        public int ErrorTypeID { get; set; }

        /// <summary> Date this error was added to the item/work </summary>
        public DateTime DateAdded { get; set; }

        /// <summary> User that added this error to the item/work </summary>
        public string AddedByUser { get; set; }

        /// <summary> Description used for the OTHER option </summary>
        public string OtherDesription { get; set; }
    }
}
