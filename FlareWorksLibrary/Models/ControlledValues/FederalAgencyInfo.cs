namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable federal agency which may be the producer of material </summary>
    public class FederalAgencyInfo
    {
        /// <summary> Text for this federal agency </summary>
        public string Agency { get; set; }

        /// <summary> Primary key for this federal agency </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="FederalAgencyInfo"/> class </summary>
        public FederalAgencyInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="FederalAgencyInfo"/> class </summary>
        /// <param name="ID"> Primary key for this federal agency </param>
        /// <param name="Agency"> Text for this federal agency </param>
        public FederalAgencyInfo(int ID, string Agency)
        {
            this.ID = ID;
            this.Agency = Agency;
        }
    }
}
