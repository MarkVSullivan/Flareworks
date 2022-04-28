using System;
using FlareWorks.Models.ControlledValues;

namespace FlareWorks.Library.Models.QC
{
    public class WorkSetItem
    {


        /// <summary> Primary key for this item work from the database </summary>
        public int PrimaryKey { get; set; }

        /// <summary> Date these items were initially added to the Flareworks database </summary>
        public DateTime DateAdded { get; set; }

        /// <summary> Number of items within this set that were sent to tray </summary>
        public int ItemsSentToTray { get; set; }

        /// <summary> Number of items within this set that were withdrawn </summary>
        public int ItemsWithdrawn { get; set; }

        /// <summary> Number of items within this set that were damaged </summary>
        public int ItemsDamaged { get; set; }

        /// <summary> Date this set of items was last updated, or NULL </summary>
        public DateTime? DateLastUpdated { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="WorkSetItem"/> class </summary>
        public WorkSetItem()
        {
            PrimaryKey = -1;
        }
    }
}
