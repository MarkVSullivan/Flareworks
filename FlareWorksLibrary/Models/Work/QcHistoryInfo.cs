using System;

namespace FlareWorks.Models.Work
{
    /// <summary> Information about a QC event for a set of items within a title </summary>
    public class QcHistoryInfo
    {
        /// <summary> Primary key to this single QC event </summary>
        public int PrimaryKey { get; set; }

        /// <summary> Flag indicates if this event includes a rejection of the material </summary>
        public bool Rejected { get; set; }

        /// <summary> User that performed this quality control check </summary>
        public string User { get; set; }

        /// <summary> Date this quality control check was submitted </summary>
        public DateTime Date { get; set; }

        /// <summary> Notes related to this quality control check </summary>
        public string Notes { get; set; }
    }
}
