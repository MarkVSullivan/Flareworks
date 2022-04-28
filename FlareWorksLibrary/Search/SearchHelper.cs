using FlareWorks.Library.Database;
using FlareWorks.Library.Models.Search;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlareWorks.Library.Search
{
    public static class SearchHelper
    {
        public static DataTable PerformSearch( SearchInfo Search )
        {
            // Collect the parameters for the search
            List<string> fields = new List<string>();
            List<int> ids = new List<int>();
            List<string> frees = new List<string>();

            foreach( SingleSearchCriterion criterion in Search.Criteria )
            {
                fields.Add(criterion.FieldCode);
                ids.Add(criterion.ControlledMatch);
                frees.Add((!String.IsNullOrEmpty(criterion.Parameter) ? criterion.Parameter : String.Empty));
            }

            // Need a total of 15 criterion
            while ( fields.Count < 15 )
            {
                fields.Add(String.Empty);
                ids.Add(-1);
                frees.Add(String.Empty);
            }

            // Now, call the database
            DataTable returnTbl = DatabaseGateway.Perform_Search(fields[0], ids[0], frees[0], fields[1], ids[1], frees[1], fields[2], ids[2], frees[2],
                fields[3], ids[3], frees[3], fields[4], ids[4], frees[4], fields[5], ids[5], frees[5], fields[6], ids[6], frees[6],
                fields[7], ids[7], frees[7], fields[8], ids[8], frees[8], fields[9], ids[9], frees[9], fields[10], ids[10], frees[10],
                fields[11], ids[11], frees[11], fields[12], ids[12], frees[12], fields[13], ids[13], frees[13], fields[14], ids[14], frees[14],
                Search.DateRange_Start, Search.DateRange_End, Search.Grouping);


            return returnTbl;


        }
    }
}
