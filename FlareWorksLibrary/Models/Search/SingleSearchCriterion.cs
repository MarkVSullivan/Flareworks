namespace FlareWorks.Library.Models.Search
{
    /// <summary> A single search criteria, which is likely a part of a larger set of search criteria </summary>
    public class SingleSearchCriterion
    {
        /// <summary> Code specifying the field to search within for this single search criteria</summary>
        public string FieldCode { get; set; }

        /// <summary> User entered search uncontrolled search parameter </summary>
        public string Parameter { get; set; }

        /// <summary> If this search criteria is across a controlled field, the primary key
        /// for the matching value in the database </summary>
        public int ControlledMatch { get; set; }

        /// <summary> Constructor for a search against an uncontrolled field </summary>
        /// <param name="FieldCode"> Code specifying the field to search within for this single search criteria </param>
        /// <param name="Parameter"> User entered search uncontrolled search parameter </param>
        public SingleSearchCriterion(string FieldCode, string Parameter)
        {
            this.FieldCode = FieldCode;
            this.Parameter = Parameter;
            this.ControlledMatch = 0;
        }

        /// <summary> Constructor for a search against a controlled field (selected by drop-down) </summary>
        /// <param name="FieldCode"> Code specifying the field to search within for this single search criteria </param>
        /// <param name="ControlledMatch"> If this search criteria is across a controlled field, the primary key
        /// for the matching value in the database </param>
        public SingleSearchCriterion( string FieldCode, int ControlledMatch )
        {
            this.FieldCode = FieldCode;
            this.ControlledMatch = ControlledMatch;

        }
    }
}
