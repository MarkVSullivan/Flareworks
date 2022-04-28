namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable material type for the material 
    /// ( i.e., Print, CD, VHS, Map, or Microform ) </summary>
    public class MaterialTypeInfo
    {
        /// <summary> Text for this material type </summary>
        public string Text { get; set; }

        /// <summary> Primary key for this material type </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="MaterialTypeInfo"/> class </summary>
        public MaterialTypeInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="MaterialTypeInfo"/> class </summary>
        /// <param name="ID"> Primary key for this material type </param>
        /// <param name="Text"> Text for this material type </param>
        public MaterialTypeInfo(int ID, string Text)
        {
            this.ID = ID;
            this.Text = Text;
        }
    }
}
