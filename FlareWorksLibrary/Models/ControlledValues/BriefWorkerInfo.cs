namespace FlareWorks.Library.Models.ControlledValues
{
    /// <summary> Class stores basic information about a user, used when we need to display the
    /// list of users in a drop-down list </summary>
    public class BriefWorkerInfo
    {
        /// <summary> Name of this worker </summary>
        public string Name { get; set; }

        /// <summary> Primary key for this worker </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="BriefWorkerInfo"/> class </summary>
        public BriefWorkerInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="BriefWorkerInfo"/> class </summary>
        /// <param name="ID"> Primary key for this worker </param>
        /// <param name="Name"> Name of this worker  </param>
        public BriefWorkerInfo(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }
    }
}
