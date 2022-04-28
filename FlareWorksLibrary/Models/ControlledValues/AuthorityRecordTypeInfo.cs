namespace FlareWorks.Library.Models.ControlledValues
{
    /// <summary> One selectable authority record type, selectable when saving authority work
    /// performed at the title level ( i.e., 'Personal Name', 'Corporate Body', 'Conference', 'Spatia', or 'Subject' ) </summary>
    public class AuthorityRecordTypeInfo
    {
        /// <summary> Text for this authority record type </summary>
        public string RecordType { get; set; }

        /// <summary> Primary key for this authority record type </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="AuthorityRecordTypeInfo"/> class </summary>
        public AuthorityRecordTypeInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="AuthorityRecordTypeInfo"/> class </summary>
        /// <param name="ID"> Primary key for this authority record type </param>
        /// <param name="RecordType"> Text for this authority record type </param>
        public AuthorityRecordTypeInfo(int ID, string RecordType)
        {
            this.ID = ID;
            this.RecordType = RecordType;
        }
    }
}
