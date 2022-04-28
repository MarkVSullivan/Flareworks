using System;
using FlareWorks.Models.ControlledValues;

namespace FlareWorks.Models.Work
{
    /// <summary> A set of items within a title that share similarities, like the time it was processed,
    /// material type, institution, and worker </summary>
    public class ItemWork
    {
        /// <summary> Source institution for these items within the title </summary>
        public InstitutionInfo Institution { get; set; }

        /// <summary> Material type ( i.e., Print, CD, VHS, Map, or Microform ) </summary>
        public MaterialTypeInfo MaterialType { get; set; }

        /// <summary> Worker who performed the ingest/work for thess items within the title </summary>
        public string Worker { get; set; }

        /// <summary> Primary key for this item work from the database </summary>
        public int PrimaryKey { get; set; }

        /// <summary> Date these items were initially added to the Flareworks database </summary>
        public DateTime DateAdded { get; set; }

        /// <summary> Date these items were submitted for QC by the worker, or NULL </summary>
        public DateTime? DateSubmitted { get; set; }

        /// <summary> Number of items within this set that were sent to tray </summary>
        public int ItemsSentToTray { get; set; }

        /// <summary> Number of items within this set that were withdrawn </summary>
        public int ItemsWithdrawn { get; set; }

        /// <summary> Number of items within this set that were damaged </summary>
        public int ItemsDamaged { get; set; }

        /// <summary> Flag indicates if this is the last copy? </summary>
        public bool LastCopy { get; set; }

        /// <summary> Date this set of items was last updated, or NULL </summary>
        public DateTime? DateLastUpdated { get; set; }

        /// <summary> Date this set was rejected through QC, or NULL </summary>
        public DateTime? DateRejected { get; set; }

        /// <summary> Date this set was approved by QC, or NULL </summary>
        public DateTime? DateApproved { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ItemWork"/> class </summary>
        public ItemWork()
        {
            PrimaryKey = -1;
        }
    }
}
