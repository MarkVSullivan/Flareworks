namespace FlareWorks.Library.Models.ControlledValues
{
    /// <summary> One selectable record type, which determines the overall type
    /// of record, such as Government or Non-Government document </summary>
    public class RecordTypeInfo
    {
        /// <summary> Text for this record type </summary>
        public string Text { get; set; }

        /// <summary> Primary key for this record type </summary>
        public int ID { get; set; }

        /// <summary> Description for this record type </summary>
        public string Description { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="RecordTypeInfo"/> class </summary>
        public RecordTypeInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="RecordTypeInfo"/> class </summary>
        /// <param name="ID"> Primary key for this record type </param>
        /// <param name="Text"> Text for this record type </param>
        /// <param name="Description"> Description for this record type </param>
        public RecordTypeInfo(int ID, string Text, string Description)
        {
            this.ID = ID;
            this.Text = Text;
            this.Description = Description;
        }

        public bool issn_row_display()
        {
            return ( ID == 2 );
        }

        public bool doctype_row_display()
        {
            return (ID == 1);
        }

        public bool fed_agency_row_display()
        {
            return (ID == 1);
        }
    }
}
