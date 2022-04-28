namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable cataloging type, which tells if the catalog record is 
    /// an original type, copy, processed, or an add </summary>
    public class CatalogingTypeInfo
    {
        /// <summary> Text for this cataloging type </summary>
        public string Text { get; set; }

        /// <summary> Primary key for this cataloging type </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="CatalogingTypeInfo"/> class </summary>
        public CatalogingTypeInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="CatalogingTypeInfo"/> class </summary>
        /// <param name="ID"> Primary key for this cataloging type </param>
        /// <param name="Text"> Text for this cataloging type </param>
        public CatalogingTypeInfo(int ID, string Text)
        {
            this.ID = ID;
            this.Text = Text;
        }
    }
}
