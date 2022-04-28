namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable document type, which provides information on the general
    /// type of the document ( i.e., Federal Government, EU, Florida, Planning, Headings ) </summary>
    public class DocumentTypeInfo
    {
        /// <summary> Text for this document type </summary>
        public string Text { get; set; }

        /// <summary> Primary key for this document type </summary>
        public int ID { get; set; }

        /// <summary> Description for this document type </summary>
        public string Description { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="DocumentTypeInfo"/> class </summary>
        public DocumentTypeInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="DocumentTypeInfo"/> class </summary>
        /// <param name="ID"> Primary key for this document type </param>
        /// <param name="Text"> Text for this document type </param>
        /// <param name="Description"> Description for this document type </param>
        public DocumentTypeInfo(int ID, string Text, string Description)
        {
            this.ID = ID;
            this.Text = Text;
            this.Description = Description;
        }
    }
}
