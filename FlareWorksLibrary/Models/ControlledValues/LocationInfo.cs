namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable location, to which users can be linked </summary>
    public class LocationInfo
    {
        /// <summary> Name of this location, to which users may be linked </summary>
        public string Name { get; set; }

        /// <summary> Code for this location, to which users may be linked </summary>
        public string Code { get; set; }

        /// <summary> Primary key for this location, to which users may be linked </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="LocationInfo"/> class </summary>
        public LocationInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="LocationInfo"/> class </summary>
        /// <param name="ID"> Primary key for this location, to which users may be linked </param>
        /// <param name="Name"> Name of this location, to which users may be linked </param>
        /// <param name="Code"> Code for this location, to which users may be linked </param>
        public LocationInfo(int ID, string Name, string Code)
        {
            this.ID = ID;
            this.Name = Name;
            this.Code = Code;
        }
    }
}
