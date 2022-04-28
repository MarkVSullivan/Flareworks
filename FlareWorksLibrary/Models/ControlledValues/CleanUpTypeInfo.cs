namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable clean up type which may need to be performed against the catalog record </summary>
    public class CleanUpTypeInfo
    {
        /// <summary> Text for this clean up type </summary>
        public string Text { get; set; }

        /// <summary> Primary key for this clean up type </summary>
        public int ID { get; set; }

        /// <summary> Description for this clean up type </summary>
        public string Description { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="CleanUpTypeInfo"/> class </summary>
        public CleanUpTypeInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="CleanUpTypeInfo"/> class </summary>
        /// <param name="ID"> Primary key for this clean up type </param>
        /// <param name="Text"> Text for this clean up type </param>
        /// <param name="Description"> Description for this clean up type </param>
        public CleanUpTypeInfo(int ID, string Text, string Description)
        {
            this.ID = ID;
            this.Text = Text;
            this.Description = Description;
        }
    }
}
