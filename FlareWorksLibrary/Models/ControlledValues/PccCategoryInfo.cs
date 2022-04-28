namespace FlareWorks.Library.Models.ControlledValues
{
    /// <summary> One selectable PCC category type, such as 'New Authentication', 
    /// 'Maintenance', or 'Non-CONSER Maintenance' </summary>
    public class PccCategoryInfo
    {
        /// <summary> Text for this PCC Category type </summary>
        public string Category { get; set; }

        /// <summary> Primary key for this PCC Category type </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="PccCategoryInfo"/> class </summary>
        public PccCategoryInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="PccCategoryInfo"/> class </summary>
        /// <param name="ID"> Primary key for this PCC Category type </param>
        /// <param name="Category"> Text for this PCC Category type </param>
        public PccCategoryInfo(int ID, string Category)
        {
            this.ID = ID;
            this.Category = Category;
        }
    }
}
