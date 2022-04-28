using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlareWorks.Library.Models.Search
{
    /// <summary> Class contains all the information about a single search </summary>
    public class SearchInfo
    {
        public List<SingleSearchCriterion> Criteria { internal set; get; }

        /// <summary> Constructor for a new empty instance of the SearchInfo class </summary>
        public SearchInfo()
        {
            Criteria = new List<SingleSearchCriterion>();
        }

        /// <summary>  Constructor for a new instance of the SearchInfo class based on the query string options </summary>
        /// <param name="QueryOptions"> List of query options from the URL </param>
        public SearchInfo(NameValueCollection QueryOptions)
        {
            Criteria = new List<SingleSearchCriterion>();

            foreach (string key in QueryOptions)
            {
                switch( key.ToUpper() )
                {
                    // Uncontrolled search criterion
                    case "TI":
                    case "IS":
                    case "OC":
                    case "AL":
                    case "FA":
                        string search_param = QueryOptions[key];
                        Add_Criteria(key.ToUpper(), search_param);
                        break;

                    // Controlled search criterion
                    case "BL":
                    case "CT":
                    case "DT":
                    case "HO":
                    case "PC":
                    case "US":
                    case "LO":
                    case "IN":
                    case "TY":
                        int controlled_id;
                        if ( Int32.TryParse(QueryOptions[key], out controlled_id))
                        {
                            Add_Criteria(key.ToUpper(), controlled_id);
                        }
                        break;

                    case "DS":
                        string date_value = QueryOptions[key];
                        DateTime test_start;
                        if (DateTime.TryParse(date_value, out test_start))
                            DateRange_Start = test_start;
                        break;

                    case "DE":
                        string date_value2 = QueryOptions[key];
                        DateTime test_end;
                        if (DateTime.TryParse(date_value2, out test_end))
                            DateRange_Start = test_end;
                        break;

                    case "GR":
                        Grouping = QueryOptions[key];
                        break;


                }
            }
        }

        /// <summary> Add a single uncontrolled search criteria to this search </summary>
        /// <param name="FieldCode"> Code specifying the field to search within for this single search criteria </param>
        /// <param name="Parameter"> User entered search uncontrolled search parameter </param>
        public void Add_Criteria( string FieldCode, string Parameter)
        {
            SingleSearchCriterion newCriteria = new SingleSearchCriterion(FieldCode, Parameter);
            Criteria.Add(newCriteria);
        }

        /// <summary> Add a single controlled search criteria to this search </summary>
        /// <param name="FieldCode"> Code specifying the field to search within for this single search criteria </param>
        /// <param name="ControlledMatch">  If this search criteria is across a controlled field, the primary key
        /// for the matching value in the database </param>
        public void Add_Criteria(string FieldCode, int ControlledMatch)
        {
            SingleSearchCriterion newCriteria = new SingleSearchCriterion(FieldCode, ControlledMatch);
            Criteria.Add(newCriteria);
        }



        /// <summary> Returns the number of criteria for this search </summary>
        /// <remarks> Can be used to see if enough criteria was added </remarks>
        public int CriteriaCount
        {
            get
            {
                return Criteria.Count;
            }
        }

        /// <summary> If there is a date range, the beginning of that range </summary>
        public DateTime? DateRange_Start { get; set; }

        /// <summary> If there is a date range, the end of that range </summary>
        public DateTime? DateRange_End { get; set; }

        /// <summary> Grouping of the results </summary>
        public string Grouping { get; set; }

        /// <summary> Get the criteria string, based on the criteria </summary>
        public string QueryString
        {
            get
            {
                // If no criteria at all, then no query string
                if ((!DateRange_Start.HasValue) && (!DateRange_End.HasValue) && (Criteria.Count == 0) && (String.IsNullOrEmpty(Grouping)))
                    return String.Empty;

                // Start to build the string
                StringBuilder queryBuilder = new StringBuilder();

                // Add each criteria
                foreach( SingleSearchCriterion criterion in Criteria )
                {
                    // Was this a controlled search or not?
                    if ( !String.IsNullOrEmpty(criterion.Parameter))
                    {
                        queryBuilder.Append("&" + criterion.FieldCode + "=" + WebUtility.UrlEncode(criterion.Parameter));
                    }
                    else if ( criterion.ControlledMatch > 0 )
                    {
                        queryBuilder.Append("&" + criterion.FieldCode + "=" + criterion.ControlledMatch);
                    }
                }

                // Add the date start and date end
                if ( DateRange_Start.HasValue )
                {
                    queryBuilder.Append("&DS=" + DateRange_Start.Value.ToShortDateString());
                }
                if (DateRange_End.HasValue)
                {
                    queryBuilder.Append("&DE=" + DateRange_End.Value.ToShortDateString());
                }

                // Add the grouping 
                if (!String.IsNullOrEmpty(Grouping))
                {
                    queryBuilder.Append("&GR=" + Grouping);
                }

                // The first & should be a ?
                queryBuilder[0] = '?';

                // Return the string
                return queryBuilder.ToString();
            }
        }

    }
}
