namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable institution, from which material may be added to FLARE </summary>
    public class InstitutionInfo
    {
        /// <summary> Name of this institution, to which material may be linked </summary>
        public string Name { get; set; }

        /// <summary> Code for this institution, to which material may be linked </summary>
        public string Code { get; set; }

        /// <summary> Primary key for this institution, to which material may be linked </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="InstitutionInfo"/> class </summary>
        public InstitutionInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="InstitutionInfo"/> class </summary>
        /// <param name="ID"> Primary key for this institution, to which material may be linked </param>
        /// <param name="Name"> Name of this institution, to which material may be linked </param>
        /// <param name="Code"> Code for this institution, to which material may be linked </param>
        public InstitutionInfo(int ID, string Name, string Code)
        {
            this.ID = ID;
            this.Name = Name;
            this.Code = Code;
        }
    }
}
