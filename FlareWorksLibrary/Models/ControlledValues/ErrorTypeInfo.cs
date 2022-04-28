namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable error type, which indicates errors that are found in 
    /// the records when quality control is performed </summary>
    public class ErrorTypeInfo
    {
        /// <summary> Text for this error type </summary>
        public string Text { get; set; }

        /// <summary> Primary key for this error type </summary>
        public int ID { get; set; }

        /// <summary> Description for this error type </summary>
        public string Description { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ErrorTypeInfo"/> class </summary>
        public ErrorTypeInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="ErrorTypeInfo"/> class </summary>
        /// <param name="ID"> Primary key for this error type </param>
        /// <param name="Text"> Text for this error type </param>
        /// <param name="Description"> Description for this error type </param>
        public ErrorTypeInfo(int ID, string Text, string Description)
        {
            this.ID = ID;
            this.Text = Text;
            this.Description = Description;
        }
    }
}
